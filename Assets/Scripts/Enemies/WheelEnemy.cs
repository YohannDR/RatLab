using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(StatisObject))]
public class WheelEnemy : Enemy
{
    private Rigidbody m_RigidBody;
    private StatisObject m_StatisObject;

    [SerializeField] private float m_SpinSpeed;

    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_StatisObject = GetComponent<StatisObject>();

        m_StatisObject.OnFreezeCallbacks += () => m_RigidBody.velocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        if (m_StatisObject.IsFreezed())
            return;

        m_RigidBody.velocity = new(m_SpinSpeed, 0, 0);
    }
}
