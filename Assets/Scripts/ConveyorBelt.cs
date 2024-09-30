using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(StatisObject))]
public class ConveyorBelt : MonoBehaviour, IWheelInteractible
{
    private enum Direction { Left, Right };

    [SerializeField] private Direction m_Direction;
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_ResistanceForce;
    [SerializeField] private bool m_NeedContinuousSpin;
    [SerializeField] private bool m_IsStopped;

    private StatisObject m_Statis;
    private Rigidbody m_RigidBody;
    private PlayerBehavior m_Player;

    private void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_Statis = GetComponent<StatisObject>();

        m_Statis.OnUnfreezeCallbacks += () => m_RigidBody.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (m_IsStopped || m_Statis.IsFreezed())
        {
            AudioManager.Instance.StopSound("ConveyorActive");
            return;
        }

        AudioManager.Instance.PlaySoundIfNotPlaying("ConveyorActive", transform.position);
        Vector3 prevPos = m_RigidBody.position;
        m_RigidBody.position += GetMovementDirection() * (m_Speed * Time.fixedDeltaTime);
        m_RigidBody.MovePosition(prevPos);
    }

    public void OnSpin(float deltaRotation)
    {
    }

    public void OnStartSpin()
    {
        m_IsStopped = false;
    }

    public void OnStopSpin()
    {
        if (m_NeedContinuousSpin)
            m_IsStopped = true;
    }

    private Vector3 GetMovementDirection()
    {
        return m_Direction == Direction.Left ? transform.forward : -transform.forward;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out m_Player))
            return;
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_Player == null)
            return;

        Vector3 direction = GetMovementDirection();
        Rigidbody rb = m_Player.GetComponent<Rigidbody>();

        if (Mathf.Sign(direction.x) != Mathf.Sign(rb.velocity.x))
            m_Player.ScaleSpeed(1.0f);
        else
            m_Player.ScaleSpeed(m_ResistanceForce);
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_Player != null)
        {
            m_Player.ScaleSpeed(1.0f);
            m_Player = null;
        }

    }
}
