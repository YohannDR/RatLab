using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(StatisBeamBehavior))]
public class PlayerBehavior : MonoBehaviour
{
    #region UnityComponents

    private Rigidbody m_RigidBody;
    private Animator m_Animator;
    private CapsuleCollider m_CapsuleCollider;

    #endregion

    #region InputFields

    private PlayerInputsControls m_InputControls;

    private Vector2 m_Movement;
    private Vector2 m_AimDirection;

    private bool m_Jumping;
    private bool m_PressedJump;

    private Vector3 m_CustomGravity;

    #endregion

    #region InspectorFields

    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_JumpForce;
    [SerializeField] private float m_FreezeRayDistance;
    [SerializeField] private float m_CoyoteTime;
    [SerializeField] private float m_GravityValue;
    [SerializeField] private float m_SpeedCap;
    [SerializeField] private float m_YSpeedCap;
    [SerializeField] private float m_HighJumpThreshold;
    [SerializeField] private float m_SmallJumpMultiplier;
    [SerializeField] private float m_HighJumpMultiplier;
    [SerializeField] private float m_AirResistance;
    [SerializeField] private Transform m_Feet;
    [SerializeField] private float m_ApexModifier;
    [SerializeField] private float m_ApexNoGravityTimer;
    [SerializeField] public byte m_Health;


    #endregion

    #region Misc

    private GameObject m_AimArrow;
    private Hammer m_Hammer;
    private StatisBeamBehavior m_StatisBeam;
    private StatisObject m_Statis;
    private float m_CoyoteCounter;

    private float m_ActualSpeedCap;
    private float m_ActualMovementSpeed;
    private bool m_IsGravityApplied;

    public Text hpText;
    public Text cheeseText;

    #endregion

    #region Sounds

    private static readonly string[] m_PlayerFootstepSounds =
    {
        "PlayerFootsteps0",
        "PlayerFootsteps1",
        "PlayerFootsteps2",
        "PlayerFootsteps3"
    };

    private static readonly string[] m_PlayerWooshSounds =
    {
        "PlayerWoosh0",
        "PlayerWoosh1",
        "PlayerWoosh2",
        "PlayerWoosh3"
    };

    private static readonly string[] m_PlayerJumpSounds =
    {
        "PlayerJump0",
        "PlayerJump1",
        "PlayerJump2"
    };

    private static readonly string[] m_PlayerDamageSounds =
    {
        "Player_damage0",
        "Player_damage1",
        "Player_damage2"
    };

    private string m_CurrentFootstepSound;

    #endregion

    #region Initialization

    void Awake()
    {
        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;

        SetupInputs();
        GetUnityComponents();
        PlayLevelMusic();

        m_AimArrow = transform.GetChild(1).gameObject;

        m_PressedJump = m_Jumping = false;
        m_IsGravityApplied = true;
        m_ActualSpeedCap = m_SpeedCap;
        m_ActualMovementSpeed = m_MovementSpeed;
    }

    private void PlayLevelMusic()
    {
    }

    private void Start()
    {
        SyncWithCheckPoint();
    }

    private void SyncWithCheckPoint()
    {
        m_Health = CheckpointManager.HealthBackup;
        if (m_Health == 0)
        {
            m_Health = 3;
            CheckpointManager.LastCheckpointPosition = Vector3.zero;
        }

        Vector3 position = CheckpointManager.LastCheckpointPosition;
        if (position == Vector3.zero)
            return;

        PlayerBehavior player = FindObjectOfType<PlayerBehavior>();
        player.transform.position = position;
    }

    private void SetupInputs()
    {
        m_InputControls = new();

        PlayerInputsControls.GameplayActions actions = m_InputControls.Gameplay;

        actions.Move.performed += OnMove;
        actions.Move.canceled += OnMove;

        actions.Aim.performed += OnAim;

        actions.Freeze.performed += OnFreeze;
        actions.Hit.performed += OnHit;

        actions.Jump.performed += OnJump;
        actions.Jump.canceled += OnJump;

        actions.Reload.performed += OnReload;

        actions.Enable();
    }

    private void GetUnityComponents()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_StatisBeam = GetComponent<StatisBeamBehavior>();
        m_CapsuleCollider = GetComponent<CapsuleCollider>();
        m_Animator = GetComponentInChildren<Animator>();
        m_Statis = GetComponent<StatisObject>();
        m_Hammer = GetComponentInChildren<Hammer>();

        m_Statis.OnFreezeCallbacks += () =>
        {
            m_InputControls.Gameplay.Disable();

            m_Movement = Vector2.zero;
        };

        m_Statis.OnUnfreezeCallbacks += () =>
        {
            m_InputControls.Gameplay.Enable();
        };
    }

    #endregion

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleAim();
        HandleGravity();
        
        hpText.text = $"{m_Health}";
        cheeseText.text = $"{CheeseManager.GetCurrentLevelCheese()}";
    }

    private void HandleMovement()
    {
        bool grounded = IsGrounded();
        float multiplier = (grounded ? 1.0f : m_AirResistance) * Time.deltaTime;

        m_Animator.SetBool("Run", grounded && m_Movement.x != 0);

        Vector3 movement = new(m_Movement.x * m_ActualMovementSpeed * multiplier,
            0.0f, 0.0f);

        m_RigidBody.AddForce(movement, ForceMode.VelocityChange);

        Vector3 velocity = m_RigidBody.velocity;
        velocity.x = Mathf.Clamp(velocity.x, -m_ActualSpeedCap, m_ActualSpeedCap);
        velocity.y = Mathf.Clamp(velocity.y, -m_YSpeedCap, m_YSpeedCap);
        m_RigidBody.velocity = velocity;

        if (m_Movement.x == 0)
            return;

        if (m_Movement.x > 0)
            transform.localScale = new(1.0f, 1.0f, 1.0f);
        else
            transform.localScale = new(-1.0f, 1.0f, 1.0f);
    }

    private void HandleJump()
    {
        bool grounded = IsGrounded();

        if (grounded)
            m_CoyoteCounter = m_CoyoteTime;
        else
            m_CoyoteCounter -= Time.deltaTime;

        if (!m_Jumping)
            return;

        if (m_CoyoteCounter > 0)
        {
            AudioManager.Instance.PlaySoundIfNotPlaying(AudioManager.GetRandomSound(m_PlayerJumpSounds));
            m_Animator.SetBool("Jump", true);

            m_RigidBody.AddForce(Vector3.up * (m_JumpForce * m_SmallJumpMultiplier));
            Vector3 velocity = m_RigidBody.velocity;
            velocity.x *= m_AirResistance;
            m_RigidBody.velocity = velocity;

            m_CoyoteCounter = 0.0f;

            StartCoroutine(HandleApex());
        }
        else
        {
            m_PressedJump = false;
        }

        m_Jumping = false;
    }

    private IEnumerator HandleApex()
    {
        m_IsGravityApplied = false;
        m_Movement = new Vector3(m_Movement.x * m_ApexModifier, m_Movement.y, 0.0f);
        yield return new WaitForSeconds(m_ApexNoGravityTimer);
        m_Movement = new Vector3(m_Movement.x / m_ApexModifier, m_Movement.y, 0.0f);
        
        m_IsGravityApplied = true;
    }

    private void CheckHeldJump()
    {
        if (m_PressedJump)
        {
            m_RigidBody.AddForce(Vector3.up * (m_JumpForce * m_HighJumpMultiplier));
            StartCoroutine(PlayLongJumpSound());
        }
    }

    private IEnumerator PlayLongJumpSound()
    {
        yield return new WaitForSeconds(0.1f);
        AudioManager.Instance.PlaySoundIfNotPlaying("PlayerJumpLong");
    }

    private void HandleGravity()
    {
        if (!m_IsGravityApplied)
            return;

        m_CustomGravity = m_RigidBody.velocity;
        m_CustomGravity.y = m_GravityValue * Physics.gravity.y * m_RigidBody.mass;

        m_RigidBody.AddForce(m_CustomGravity);
    }

    private bool IsGrounded()
    {
        Vector3 size = new(m_CapsuleCollider.radius * 2.0f - 0.3f, 0.1f, m_CapsuleCollider.radius);
        Collider[] colliders = Physics.OverlapBox(m_Feet.position, size);

        bool grounded = false;
        float threshold = m_CapsuleCollider.radius;
        Vector3 position = transform.position;
        foreach (Collider c in colliders)
        {
            Vector3 colPosition = c.bounds.center;

            if (c.gameObject == gameObject)
                continue;

            // Check is below
            if (position.y <= colPosition.y)
                continue;

            float colThreshold = c.bounds.size.x / 2.0f;

            float x1 = position.x - threshold;
            float x2 = position.x + threshold;

            float y1 = colPosition.x - colThreshold;
            float y2 = colPosition.x + colThreshold;

            // Range check on the X axis between the player collider and the hit object collider
            // Basically check if [-playerX ; playerX] and [-colliderX ; colliderX] overlap
            if (Mathf.Max(x1, y1) <= Mathf.Min(x2, y2))
            {
                grounded = true;
                break;
            }
        }

        return grounded;
    }


    private void HandleAim()
    {
        bool active = m_AimDirection.x + m_AimDirection.y != 0;

        m_AimArrow.SetActive(active);

        if (!active)
            return;

        m_AimArrow.transform.position = transform.position + new Vector3(2.25f, 0.0f, 0.0f);

        float angle = MathF.Atan2(m_AimDirection.y, m_AimDirection.x) * Mathf.Rad2Deg;
        m_AimArrow.transform.RotateAround(transform.position, Vector3.forward, angle);
        m_AimArrow.transform.rotation = Quaternion.Euler(-angle, 90.0f, 0.0f);
    }

    public void HandleFreeze()
    {
        m_Animator.SetBool("Shoot", true);
        StasisParticlesManager.Instance.PlayStasisBeam(transform.position, m_AimDirection);
        m_StatisBeam.ShootBeam(m_AimDirection, m_FreezeRayDistance);
    }

    public void ScaleSpeed(float scale)
    {
        m_ActualMovementSpeed = m_MovementSpeed * scale;
        m_ActualSpeedCap = m_SpeedCap * scale;
    }

    public void Kill()
    {
        AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_PlayerDamageSounds));
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        m_Animator.SetBool("Death", true);
        m_Health--;

        yield return new WaitForSeconds(1.0f);

        CheckpointManager.LoadLastCheckpoint(m_Health);
    }

    #region InputCallbacks
    private void OnMove(InputAction.CallbackContext ctx)
    {
        if (m_Statis.IsFreezed())
        {
            m_Movement = Vector2.zero;
            return;
        }

        m_Movement = ctx.ReadValue<Vector2>();

        if (ctx.canceled)
        {
            AudioManager.Instance.StopSound(m_CurrentFootstepSound);
            m_CurrentFootstepSound = string.Empty;
            return;
        }

        if (m_CurrentFootstepSound == string.Empty)
        {
            m_CurrentFootstepSound = AudioManager.GetRandomSound(m_PlayerFootstepSounds);
        }

        AudioManager.Instance.PlaySoundIfNotPlaying(m_CurrentFootstepSound);

    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            m_PressedJump = m_Jumping = true;
            Invoke(nameof(CheckHeldJump), 0.1f);
        }

        if (ctx.phase == InputActionPhase.Canceled)
            m_PressedJump = m_Jumping = false;
    }

    private void OnAim(InputAction.CallbackContext ctx)
    {
        m_AimDirection = ctx.ReadValue<Vector2>();
    }

    private void OnHit(InputAction.CallbackContext ctx)
    {
        AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_PlayerWooshSounds));
        m_Animator.SetBool("Hit", true);
        m_Hammer.Hit();
    }

    private void OnFreeze(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        m_Animator.SetBool("Shoot", true);
        HandleFreeze();
    }

    private void OnReload(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            CheckpointManager.LoadLastCheckpoint(m_Health);
    }

    public bool PressingUp() => m_Movement.y > 0.5f;
    public bool PressingDown() => m_Movement.y < -0.5f;

    #endregion
}
