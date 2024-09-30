using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class StatisObject : MonoBehaviour
{
    #region UnityComponents

    private Rigidbody m_RigidBody;

    #endregion

    private bool m_Freezed;
    private bool m_PermaFreezed;
    private float m_FreezeTimer;
    [SerializeField] private uint m_AccumulatedForce;
    private Vector3 m_ForcePosition;
    private bool m_CanBeFreezed;

    private static readonly string[] m_StasisReleaseSounds =
    {
        "StasisFail",
        "StasisRelease0",
        "StasisRelease1",
        "StasisRelease2"
    };

    [SerializeField] private bool m_FreezedOnLoad;
    [SerializeField] private float m_FreezeDuration;
    [SerializeField] private bool m_CanAccumulateForce;
    [SerializeField] private float[] m_Power;

    public delegate void StasisDelegate();

    public StasisDelegate OnFreezeCallbacks;
    public StasisDelegate OnUnfreezeCallbacks;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();

        m_CanBeFreezed = true;

        if (m_FreezedOnLoad)
        {
            m_PermaFreezed = true;
            SetFreezeState(true);
            OnFreezeCallbacks?.Invoke();
        }
    }

    private void Update()
    {
        if (!m_Freezed || m_PermaFreezed)
            return;

        m_FreezeTimer -= Time.deltaTime;
        if (m_FreezeTimer <= 0)
        {
            SetFreezeState(false);
        }
    }

    public void SetFreezeState(bool freezed)
    {
        if (!m_CanBeFreezed)
            return;

        m_Freezed = freezed;
        m_RigidBody.isKinematic = freezed;

        if (freezed)
            Freeze();
        else
            Unfreeze();
    }

    public void AccumulateForce(Vector3 point)
    {
        if (!m_CanAccumulateForce)
            return;

        if (m_ForcePosition != Vector3.zero)
        {
            // Use the point in the middle of the last and current force points
            m_ForcePosition = (m_ForcePosition + point) / 2.0f;
        }
        else
        {
            m_ForcePosition = point;
        }

        StasisParticlesManager.Instance.PlayStasisHit(point, m_AccumulatedForce);

        if (m_AccumulatedForce < 3)
            m_AccumulatedForce++;
    }

    private void Freeze()
    {
        m_FreezeTimer = m_FreezeDuration;

        if (!m_PermaFreezed)
        {
            AudioManager.Instance.PlaySound("StasisHit", transform.position);
            StasisParticlesManager.Instance.PlayStasing(transform.position);
        }

        // Invoke freeze callbacks
        OnFreezeCallbacks?.Invoke();
    }

    private void Unfreeze()
    {
        m_ForcePosition.y = transform.position.y;
        m_ForcePosition.z = 0;

        if (m_CanAccumulateForce)
        {
            // Vector that goes from the point to the center of the object
            Vector3 direction = (transform.position - m_ForcePosition).normalized;

            // Apply the accumulated force
            m_RigidBody.AddForceAtPosition(direction * m_Power[m_AccumulatedForce],
                m_ForcePosition, ForceMode.VelocityChange);

            StasisParticlesManager.Instance.PlayStasisRelease(m_ForcePosition, m_AccumulatedForce);
        }

        // Play the sound
        AudioManager.Instance.PlaySound(m_StasisReleaseSounds[m_AccumulatedForce], transform.position);

        // Reset accumulated force
        m_AccumulatedForce = 0;
        m_ForcePosition = Vector3.zero;

        // Reset freeze timer
        m_FreezeTimer = 0;
        m_PermaFreezed = false;

        // Invoke unfreeze callbacks
        OnUnfreezeCallbacks?.Invoke();
    }

    public bool IsFreezed() => m_Freezed;

    public bool ToggleEnable(bool status) => m_CanBeFreezed = status;
}
