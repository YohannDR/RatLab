using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StatisObject))]
public class Slime : MonoBehaviour
{
    private BoxCollider m_BoxCollider;
    private StatisObject m_Statis;
    [SerializeField] private float m_Bounciness;

    void Awake()
    {
        m_Statis = GetComponent<StatisObject>();
        m_BoxCollider = GetComponentInChildren<BoxCollider>();

        m_Statis.OnFreezeCallbacks += () => m_BoxCollider.material.bounciness = 0;
        m_Statis.OnUnfreezeCallbacks += () => m_BoxCollider.material.bounciness = 1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check is freezed
        if (m_Statis.IsFreezed())
            return;

        // Not freezed, try to get the rigid body of the collider
        if (!collision.gameObject.TryGetComponent(out Rigidbody rb))
            return;

        AudioManager.Instance.PlaySound("Slime_rebound", transform.position);

        // Apply an upward force
        Vector3 velocity = rb.velocity;
        velocity.x = 0.0f;
        rb.velocity = velocity;
    }
}
