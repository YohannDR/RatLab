using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StatisObject), typeof(BoxCollider))]
public class BossHand : MonoBehaviour
{
    private enum BossHandPose
    {
        Idle,
        GoingDown,
        Swiping,
        GoingUp,
        WaitingAfterGoingUp,
        Dead
    }

    private BossManager m_Manager;
    private StatisObject m_Statis;
    private BoxCollider m_BoxCollider;
    private Vector3 m_SpawnPosition;

    private BossHandPose m_Pose;

    private float m_SwipingDirection;
    private float m_SwipingRangeDone;

    private float m_IntervalTimer;

    [SerializeField] private ushort m_Health;
    [SerializeField] private float m_FallingSpeed;
    [SerializeField] private float m_SwipingSpeed;
    [SerializeField] private float m_SwipingRange;
    [SerializeField] private float m_GoingUpSpeed;
    [SerializeField] private float m_AttackInterval;

    void Awake()
    {
        m_Manager = FindObjectOfType<BossManager>();
        m_BoxCollider = GetComponent<BoxCollider>();
        m_SpawnPosition = transform.position;

        m_Statis = GetComponent<StatisObject>();

        SetPose(BossHandPose.Idle);
    }

    void FixedUpdate()
    {
        if (m_Statis.IsFreezed())
            return;

        switch (m_Pose)
        {
            case BossHandPose.Idle:
                if (m_Manager.ShouldAttack())
                {
                    UpdateIdleMovement();
                }
                else
                {
                    // TODO player detection
                    SetPose(BossHandPose.GoingDown);
                }
                break;

            case BossHandPose.GoingDown:
                FallDown();
                break;

            case BossHandPose.Swiping:
                Swipe();
                break;

            case BossHandPose.GoingUp:
                GoingUp();
                break;

            case BossHandPose.WaitingAfterGoingUp:
                UpdateAttackInterval();
                break;

            case BossHandPose.Dead:
                break;
        }
    }

    private void SetPose(BossHandPose pose)
    {
        m_Pose = pose;

        switch (pose)
        {
            case BossHandPose.Idle:
            case BossHandPose.GoingUp:
                m_Statis.ToggleEnable(false);
                break;

            case BossHandPose.Swiping:
                m_Statis.ToggleEnable(true);

                m_SwipingDirection = GetSwipingDirection();
                m_SwipingRangeDone = 0.0f;
                // transform.rotation = Quaternion.Euler(90 * m_SwipingDirection, 0, 0);
                break;

            case BossHandPose.WaitingAfterGoingUp:
                m_IntervalTimer = m_AttackInterval;
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 position = transform.position;
        position.y -= m_FallingSpeed * Time.fixedDeltaTime;

        m_BoxCollider = GetComponent<BoxCollider>();
        Vector3 size = new Vector3(0.0f, m_BoxCollider.size.y / 2.0f, 0.0f) * 0.1f;
        Gizmos.DrawLine(position, position - size);
    }

    private void FallDown()
    {
        Vector3 position = transform.position;
        position.y -= m_FallingSpeed * Time.fixedDeltaTime;

        Vector3 size = new(0.0f, m_BoxCollider.size.y / 2.0f, 0.0f);

        if (Physics.Raycast(position - size,
            Vector3.down, out RaycastHit hit, 0.1f))
        {
            position.y = hit.point.y + m_BoxCollider.size.y / 2.0f;

            SetPose(BossHandPose.Swiping);
        }

        transform.position = position;
    }

    private void Swipe()
    {
        float velocity = m_SwipingSpeed * m_SwipingDirection * Time.fixedDeltaTime;

        Vector3 position = transform.position;
        position.x += velocity;
        m_SwipingRangeDone += Mathf.Abs(velocity);

        if (m_SwipingRangeDone >= m_SwipingRange)
        {
            SetPose(BossHandPose.GoingUp);
        }

        transform.position = position;
    }

    private void GoingUp()
    {
        Vector3 position = transform.position;
        position.y += m_GoingUpSpeed * Time.fixedDeltaTime;

        if (position.y >= m_SpawnPosition.y)
        {
            position.y = m_SpawnPosition.y;
            SetPose(BossHandPose.WaitingAfterGoingUp);
        }

        transform.position = position;
    }

    private void UpdateAttackInterval()
    {
        m_IntervalTimer -= Time.fixedDeltaTime;
        if (m_IntervalTimer <= 0)
        {
            SetPose(BossHandPose.Idle);
        }
    }

    private float GetSwipingDirection()
    {
        Vector3 playerPos = FindObjectOfType<PlayerBehavior>().transform.position;

        if (playerPos.x <= transform.position.x)
            return -1;
        else
            return 1;
    }

    private void UpdateIdleMovement()
    {
        float time = Time.realtimeSinceStartup;

        Vector3 offset = new Vector3(Mathf.Cos(time) / 2.0f, Mathf.Sin(time - 1), 0.0f) / 3.0f;

        transform.position = m_SpawnPosition + offset;
    }

    public void TakeDamage()
    {
        m_Health--;
        if (m_Health == 0)
        {
            m_Manager.RegisterHandKilled(this);
            SetPose(BossHandPose.Dead);
            // TODO proper destroy
            Destroy(gameObject);
        }
    }
}
