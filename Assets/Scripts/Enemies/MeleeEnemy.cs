using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[RequireComponent(typeof(NavMeshAgent), typeof(ConeVisionBehavior))]
public class MeleeEnemy : MonoBehaviour
{
    [SerializeField] private Transform[] m_Waypoints;

    private NavMeshAgent m_Agent;
    private ConeVisionBehavior m_ConeVisionBehavior;
    private uint m_CurrentWayPoint;
    private GameObject m_Player;
    private StatisObject m_StatisObject;

    void Awake()
    {
        Assert.IsTrue(m_Waypoints.Length != 0);

        m_Agent = GetComponent<NavMeshAgent>();
        m_CurrentWayPoint = 0;
        m_Agent.SetDestination(m_Waypoints[0].position);

        m_ConeVisionBehavior = GetComponent<ConeVisionBehavior>();

        SetupStatisObject();
    }

    private void SetupStatisObject()
    {
        if (!TryGetComponent(out m_StatisObject))
            return;

        m_StatisObject.OnFreezeCallbacks += OnFreezeCallback;
        m_StatisObject.OnUnfreezeCallbacks += OnUnfreezeCallback;
    }

    void Update()
    {
        if (m_Player != null)
        {
            FollowPlayer();
            return;
        }

        FollowWaypoints();
    }

    private void FollowWaypoints()
    {
        float distance = (m_Agent.pathEndPosition - transform.position).sqrMagnitude;
        if (distance > 1.0f)
            return;

        m_CurrentWayPoint = (m_CurrentWayPoint + 1) % (uint)m_Waypoints.Length;
        m_Agent.SetDestination(m_Waypoints[m_CurrentWayPoint].position);
    }

    private void FollowPlayer()
    {
        m_Agent.SetDestination(m_Player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (!go.CompareTag("Player"))
            return;

        m_Player = go;
        m_ConeVisionBehavior.Disable();
    }

    private void OnFreezeCallback()
    {
        if (m_Player == null)
            m_ConeVisionBehavior.Disable();

        m_Agent.isStopped = true;
    }

    private void OnUnfreezeCallback()
    {
        if (m_Player == null)
            m_ConeVisionBehavior.Enable();

        m_Agent.isStopped = false;
    }
}
