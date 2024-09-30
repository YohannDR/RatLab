using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Wheel : MonoBehaviour
{
    [SerializeField] private GameObject[] m_BoundObjects;
    [SerializeField] private GameObject m_EntityInWheel;
    [SerializeField] private float m_Offset;
    private Rigidbody m_EntityInWheelRigidBody;
    private PlayerBehavior m_Player;
    private List<IWheelInteractible> m_EntitiesWheelInterface;

    private float m_PreviousDelta;
    private bool m_PreventOnStopCall;

    void Awake()
    {
        CheckBoundObjectValidity();
        if (m_EntityInWheel != null)
            BindEntity(m_EntityInWheel);
    }

    private void CheckBoundObjectValidity()
    {
        m_EntitiesWheelInterface = new();
        m_EntitiesWheelInterface.Clear();
        if (m_BoundObjects.Length == 0)
        {
            Debug.LogWarning("No object was assigned to the wheel");
            return;
        }

        for (int i = 0; i < m_BoundObjects.Length; i++)
        {
            IWheelInteractible _interface = m_BoundObjects[i]?.GetComponent<IWheelInteractible>();
            m_EntitiesWheelInterface.Add(_interface);
            if (_interface == null)
            {
                Debug.LogError($"Object number {i} of the wheel is not compatible");
                return;
            }
        }
    }

    private void BindEntity(GameObject entity)
    {
        // We need the rigid body for the movement detection
        if (!entity.TryGetComponent(out m_EntityInWheelRigidBody))
            return;

        m_EntityInWheel = entity;
        entity.transform.position = transform.position;
        m_PreventOnStopCall = true;
    }

    void Update()
    {
        if (m_EntityInWheel == null)
            return;

        Vector3 offset = new(0.0f, m_Offset, 0.0f);
        m_EntityInWheel.transform.position = transform.position + offset;

        float deltaRotation = m_EntityInWheelRigidBody.velocity.x;
        transform.Rotate(0, 0, deltaRotation);

        if (deltaRotation == 0)
        {
            if (!m_PreventOnStopCall)
            {
                m_EntitiesWheelInterface.ForEach((e) => e.OnStopSpin());
                m_PreventOnStopCall = true;
            }
        }
        else if (m_PreviousDelta == 0)
        {
            m_PreventOnStopCall = false;
            m_EntitiesWheelInterface.ForEach((e) => e.OnStartSpin());
        }

        m_PreviousDelta = deltaRotation;
        m_EntitiesWheelInterface.ForEach((e) => e.OnSpin(deltaRotation));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_Player != null)
            return;

        if (!other.gameObject.TryGetComponent(out m_Player))
            return;
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Player != null && m_EntityInWheel == m_Player.gameObject)
        {
            if (m_Player.PressingDown())
            {
                m_EntityInWheel = null;
                m_Player = null;
            }
        }

        if (m_Player == null || m_EntityInWheel != null)
            return;

        if (m_Player.PressingUp())
            BindEntity(m_Player.gameObject);
    }
}
