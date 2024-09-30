using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class DepthPlatform : MonoBehaviour, IWheelInteractible
{
    [SerializeField] private bool m_AutomaticMovement;
    [SerializeField] private float m_OverflowTowardsCamera;

    [ShowIf("m_AutomaticMovement"), SerializeField] private bool m_StartInFront;
    [ShowIf("m_AutomaticMovement"), SerializeField] private float m_Speed;
    [ShowIf("m_AutomaticMovement"), SerializeField] private float m_Interval;
    [ShowIf("m_AutomaticMovement"), SerializeField] private float m_StartDelay;

    [HideIf("m_AutomaticMovement"), Tooltip("The amount of \"energy\" from the wheel required to start opening"), SerializeField]
    private float m_StartThreshold;
    [HideIf("m_AutomaticMovement"), Tooltip("The speed at which the platform moves"), SerializeField]
    private float m_MovingSpeed;
    [HideIf("m_AutomaticMovement"), Tooltip("The delay before the platform starts to auto-close"), SerializeField]
    private float m_DelayBeforeClosing;
    [HideIf("m_AutomaticMovement"), Tooltip("The speed at which the platform auto-closes when the wheel isn't spinning"), SerializeField]
    private float m_ClosingSpeed;
    [HideIf("m_AutomaticMovement"), Tooltip("Whether or not the platform spins back and forth"), SerializeField]
    private bool m_CycleBack;

    private float m_FrontPositionLimit;
    private float m_BackPositionLimit;

    private float m_IntervalTimer;
    private bool m_MovingDirection;

    private float m_AccumulatedSpin;
    private float m_ClosingTimer;

    public void OnSpin(float deltaRotation)
    {
        deltaRotation = Mathf.Abs(deltaRotation);
        m_AccumulatedSpin += deltaRotation;
        if (m_AccumulatedSpin < m_StartThreshold)
            return;

        Vector3 position = transform.position;
        float speed = deltaRotation * m_MovingSpeed * Time.fixedDeltaTime;

        if (m_CycleBack)
        {
            float limit;
            bool done;

            if (m_MovingDirection)
            {
                AudioManager.Instance.PlaySoundIfNotPlaying("Plateforme_in", transform.position);
                // Going forward
                position.z -= speed;
                limit = m_FrontPositionLimit;
                done = position.z <= limit;
            }
            else
            {
                AudioManager.Instance.PlaySoundIfNotPlaying("Plateforme_out", transform.position);
                // Going backwards
                position.z += speed;
                limit = m_BackPositionLimit;
                done = position.z >= limit;
            }

            if (done)
            {
                position.z = limit;
                m_MovingDirection ^= true;
            }
        }
        else
        {
            position.z -= speed;

            if (position.z <= m_FrontPositionLimit)
            {
                position.z = m_FrontPositionLimit;
            }
        }

        transform.position = position;
    }

    public void OnStartSpin()
    {
        m_ClosingTimer = float.PositiveInfinity;
    }

    public void OnStopSpin()
    {
        AudioManager.Instance.StopSound("Plateforme_in");
        AudioManager.Instance.StopSound("Plateforme_out");
        m_ClosingTimer = m_DelayBeforeClosing;
    }

    void Awake()
    {
        float halfSize = transform.localScale.z / 2.0f;
        float baseZ = transform.position.z;
        m_FrontPositionLimit = baseZ + halfSize - 1.0f - m_OverflowTowardsCamera;
        m_BackPositionLimit = baseZ + halfSize + 1.0f;
        m_IntervalTimer = m_StartDelay;

        Vector3 position = transform.position;
        if (m_StartInFront)
        {
            m_MovingDirection = false; // Going back
            position.z = m_FrontPositionLimit;
        }
        else
        {
            m_MovingDirection = true; // Going forward
            position.z = m_BackPositionLimit;
        }
        transform.position = position;
    }

    void FixedUpdate()
    {
        if (m_AutomaticMovement)
            UpdateAutoMovement();
        else
            UpdateWheelMovement();
    }

    private void UpdateAutoMovement()
    {
        if (!UpdateIntervalTimer())
            return;

        HandleAutoMovement();
    }

    private void UpdateWheelMovement()
    {
        AudioManager.Instance.StopSound("Plateforme_in");
        AudioManager.Instance.StopSound("Plateforme_out");

        m_ClosingTimer -= Time.fixedDeltaTime;
        if (m_ClosingTimer <= 0)
        {
            m_ClosingTimer = 0;
        
            Vector3 position = transform.position;
            position.z += m_ClosingSpeed * Time.fixedDeltaTime;
            if (position.z >= m_BackPositionLimit)
            {
                position.z = m_BackPositionLimit;
                m_ClosingTimer = float.PositiveInfinity;
                m_IntervalTimer = 0;
                m_AccumulatedSpin = 0;
            }

            transform.position = position;
        }
    }

    private bool UpdateIntervalTimer()
    {
        if (m_IntervalTimer <= 0)
            return true;

        m_IntervalTimer -= Time.fixedDeltaTime;
        return false;
    }

    private void HandleAutoMovement()
    {
        float limit;
        bool done;
        float speed = m_Speed * Time.fixedDeltaTime;
        Vector3 position = transform.position;
        if (m_MovingDirection)
        {
            AudioManager.Instance.PlaySoundIfNotPlaying("Plateforme_in", transform.position);
            // Going forward
            position.z -= speed;
            limit = m_FrontPositionLimit;
            done = position.z <= limit;
        }
        else
        {
            AudioManager.Instance.PlaySoundIfNotPlaying("Plateforme_out", transform.position);
            // Going backwards
            position.z += speed;
            limit = m_BackPositionLimit;
            done = position.z >= limit;
        }

        if (done)
        {
            AudioManager.Instance.StopSound("Plateforme_in");
            AudioManager.Instance.StopSound("Plateforme_out");

            position.z = limit;
            m_MovingDirection ^= true;
            m_IntervalTimer = m_Interval;
        }

        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerBehavior player))
        {
            player.Kill();
            return;
        }

        Destroy(other.gameObject);
    }
}
