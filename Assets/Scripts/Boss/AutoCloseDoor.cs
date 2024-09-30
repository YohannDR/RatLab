using UnityEngine;

public class AutoCloseDoor : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    private bool m_IsClosing;
    private float m_FrontPositionLimit;

    private void Awake()
    {
        float halfSize = transform.localScale.z / 2.0f;
        m_FrontPositionLimit = halfSize - 1.0f;

        Vector3 position = transform.position;
        position.z = halfSize + 1.0f;
        transform.position = position;
    }

    private void FixedUpdate()
    {
        if (!m_IsClosing)
            return;

        Vector3 position = transform.position;
        position.z -= m_Speed * Time.fixedDeltaTime;
        if (position.z <= m_FrontPositionLimit)
        {
            position.z = m_FrontPositionLimit;
            m_IsClosing = false;
        }

        transform.position = position;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (!go.CompareTag("Player"))
            return;

        m_IsClosing = true;
    }
}
