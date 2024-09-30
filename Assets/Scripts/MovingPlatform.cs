using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private StatisObject m_StatisObject;
    private bool m_Active;

    private Vector3 m_StartPoint;
    [SerializeField] private Vector3 m_EndPoint;
    [SerializeField, Range(0, 10)] private float m_Speed;
    [SerializeField] private float m_WaitTime;

    private List<Transform> m_ObjectsOnPlatform = new();
    
    private float m_CurrentWaitTimer;
    private byte m_CurrentDestination;

    void Awake()
    {
        m_StartPoint = transform.position;
        m_CurrentDestination = 0;
        m_Active = true;

        SetupStatisObject();
    }

    private void SetupStatisObject()
    {
        if (!TryGetComponent(out m_StatisObject))
            return;

        m_StatisObject.OnFreezeCallbacks += () => m_Active = false;
        m_StatisObject.OnUnfreezeCallbacks += () => m_Active = true;
    }

    void Update()
    {
        if (!m_Active)
            return;

        if (m_CurrentWaitTimer >= 0)
        {
            m_CurrentWaitTimer -= Time.deltaTime;
            return;
        }

        Vector3 destination = transform.position;
        if (m_CurrentDestination == 0)
        {
            destination = m_EndPoint;
        }
        else if (m_CurrentDestination == 1)
        {
            destination = m_StartPoint;
        }

        Vector3 direction = destination - transform.position;
        float distance = direction.sqrMagnitude;
        if (distance < 0.5f)
        {
            m_CurrentDestination = (byte)((m_CurrentDestination + 1) % 2);

            m_CurrentWaitTimer = m_WaitTime;
            return;
        }

        Vector3 prevPosition = transform.position;
        transform.position += direction.normalized * (m_Speed * Time.deltaTime);

        Vector3 movement = transform.position - prevPosition;

        foreach (Transform t in m_ObjectsOnPlatform)
            t.position += movement;
    }

    private void OnTriggerEnter(Collider other)
    {
        m_ObjectsOnPlatform.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        m_ObjectsOnPlatform.Remove(other.transform);
    }
}
