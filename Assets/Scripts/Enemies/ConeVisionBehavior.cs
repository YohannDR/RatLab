using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeVisionBehavior : MonoBehaviour
{
    private GameObject m_ConeVisionObject;
    private ConeCollider m_ConeCollider;

    [Header("Cone detection")]
    [SerializeField, Range(0, 180),
        Tooltip("The angular range of the cone detection")]
    private float m_ScanningRange;
    [SerializeField, Range(.1f, 5.0f),
        Tooltip("The angular speed at which the cone will rotate in its range")]
    private float m_ScanningSpeed;
    [SerializeField, Range(0, 5),
        Tooltip("The time (in seconds) that the cone will wait when reaching an extremum of its range")]
    private float m_ScanningStopTimer;

    private float m_ScanningAngleSign;
    private float m_ScanningCurrentStopTimer;
    private float m_ScanningCurrentTotalRotation;

    void Awake()
    {
        // Hopefully always the first child
        m_ConeVisionObject = transform.GetChild(0).gameObject;
        m_ConeCollider = m_ConeVisionObject.GetComponent<ConeCollider>();
        m_ScanningAngleSign = -1.0f; // Going up

        float halfConeAngle = m_ConeCollider.GetAngle() / 2.0f;
        float halfAngle = m_ScanningRange / 2.0f;

        m_ConeVisionObject.transform.Rotate(halfAngle - halfConeAngle, 0, 0);
    }

    void Update()
    {

        if (m_ScanningCurrentStopTimer > 0)
        {
            m_ScanningCurrentStopTimer -= Time.deltaTime;
            return;
        }

        float totalDist = m_ScanningRange - m_ConeCollider.GetAngle();

        Transform coneTransform = m_ConeVisionObject.transform;
        float prevX = coneTransform.rotation.eulerAngles.x;
        if (prevX > 180.0f)
            prevX -= 360.0f;

        coneTransform.Rotate(m_ScanningSpeed * m_ScanningAngleSign, 0, 0);
        Vector3 rotation = coneTransform.rotation.eulerAngles;
        float rotationX = rotation.x;
        if (rotationX > 180.0f)
            rotationX -= 360.0f;

        m_ScanningCurrentTotalRotation += Mathf.Abs(prevX - rotationX);

        if (m_ScanningCurrentTotalRotation > totalDist)
        {
            m_ScanningAngleSign = -m_ScanningAngleSign;
            m_ScanningCurrentStopTimer = m_ScanningStopTimer;
            m_ScanningCurrentTotalRotation = 0.0f;
        }
    }

    private void OnDrawGizmos()
    {
        GameObject visionObject = transform.GetChild(0).gameObject;
        Gizmos.color = Color.red;
        Vector3 pos = visionObject.transform.position;
        Vector3 forward = visionObject.transform.TransformDirection(Vector3.forward * 5);

        float halfAngle = m_ScanningRange / 2.0f;
        Vector3 upEnd = Quaternion.AngleAxis(halfAngle, Vector3.forward) * forward;
        Vector3 lowEnd = Quaternion.AngleAxis(-halfAngle, Vector3.forward) * forward;

        Gizmos.DrawLine(pos, pos + upEnd);
        Gizmos.DrawLine(pos, pos + lowEnd);
    }

    public void Disable()
    {
        enabled = false;
        m_ConeCollider.gameObject.SetActive(false);
    }

    public void Enable()
    {
        enabled = true;
        m_ConeCollider.gameObject.SetActive(true);
    }
}
