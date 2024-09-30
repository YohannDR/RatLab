using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[RequireComponent(typeof(SphereCollider), typeof(StatisBeamBehavior))]
public class FreezeEnemy : Enemy
{
    private static readonly string[] m_EnemyShootLaserSounds =
    {
        "ennemy_shoot_laser0",
        "ennemy_shoot_laser1",
        "ennemy_shoot_laser2"
    };

    private StatisBeamBehavior m_StatisBeam;
    
    [SerializeField] private bool m_HasCycles;

    [ShowIf("m_HasCycles"), SerializeField]
    private float m_ShootInterval;
    [ShowIf("m_HasCycles"), SerializeField]
    private Vector2 m_ShootDirection;
    [ShowIf("m_HasCycles"), SerializeField]
    private float m_FreezeRayDistance;

    [HideIf("m_HasCycles"), SerializeField] private float m_DetectionRange;

    private float m_ShootTimer;
    private StatisObject m_StasisObject;

    private void OnValidate()
    {
        SphereCollider sc = GetComponent<SphereCollider>();
        sc.enabled = !m_HasCycles;
        if (sc.enabled)
            sc.radius = m_DetectionRange;
        else
            sc.radius = 0;
    }

    private void OnDrawGizmos()
    {
        if (!m_HasCycles)
            return;

        Gizmos.color = Color.yellow;
        Vector3 direction = new Vector3(m_ShootDirection.x, m_ShootDirection.y, 0).normalized * 5;
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }

    void Awake()
    {
        m_StatisBeam = GetComponent<StatisBeamBehavior>();
        m_StasisObject = GetComponent<StatisObject>();
    }

    void Update()
    {
        if (m_HasCycles)
            UpdateCycle();
    }

    private void UpdateCycle()
    {
        if (m_StasisObject != null && m_StasisObject.IsFreezed())
            return;

        m_ShootTimer += Time.deltaTime;
        if (m_ShootTimer > m_ShootInterval)
        {
            m_ShootTimer = 0;

            AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_EnemyShootLaserSounds), transform.position);
            Shoot(m_ShootDirection, m_FreezeRayDistance);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_HasCycles)
            return;

        AudioManager.Instance.PlaySound("Ennemy_detect_player");
        AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_EnemyShootLaserSounds), transform.position);
        Vector2 direction = (other.transform.position - transform.position).normalized;
        Shoot(direction, m_DetectionRange);
    }

    private void Shoot(Vector2 direction, float range)
    {
        StasisParticlesManager.Instance.PlayStasisBeamEnemy(transform.position, direction);
        m_StatisBeam.ShootBeam(direction, range);
    }
}
