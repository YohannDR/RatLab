using NaughtyAttributes;
using UnityEngine;

public class SlidingDoor : MonoBehaviour, IWheelInteractible
{
    private enum Direction { Up, Down }

    [SerializeField] private Direction m_Direction;
    [SerializeField] private float m_Speed;
    [SerializeField] private bool m_LockWhenFullyOpened;
    [SerializeField] private bool m_AutoClose;

    [ShowIf("m_AutoClose"), SerializeField] private float m_ClosingSpeed;
    [ShowIf("m_AutoClose"), SerializeField] private float m_DelayBeforeClosing;

    private float m_Size;
    private float m_CurrentOffset;
    private Vector3 m_StartPosition;
    private float m_TimerBeforeClosing;
    private bool m_Locked;
    private bool m_AutoClosing;

    public void OnSpin(float deltaRotation)
    {
        if (m_Locked)
            return;

        AudioManager.Instance.PlaySoundIfNotPlaying("Door_close", transform.position);
        deltaRotation = Mathf.Abs(deltaRotation) / 500.0f * m_Speed;

        float limit;
        bool done;
        if (m_Direction == Direction.Down)
        {
            m_CurrentOffset -= deltaRotation;
            limit = -m_Size;
            done = m_CurrentOffset <= limit;
        }
        else
        {
            m_CurrentOffset += deltaRotation;
            limit = m_Size;
            done = m_CurrentOffset >= limit;
        }

        if (done)
        {
            m_CurrentOffset = limit;
            if (m_LockWhenFullyOpened)
                m_Locked = true;
        }

        ApplyMovementOffset();
    }

    public void OnStartSpin()
    {
        m_TimerBeforeClosing = float.PositiveInfinity;
    }

    public void OnStopSpin()
    {
        AudioManager.Instance.StopSound("Door_close");
        AudioManager.Instance.StopSound("Door_open");
        m_TimerBeforeClosing = m_DelayBeforeClosing;
    }

    void Awake()
    {
        m_StartPosition = transform.position;
        m_Size = transform.GetChild(0).transform.localScale.y;
    }

    void Update()
    {
        if (m_Locked)
            return;

        UpdateAutoClosingTimer();
        UpdateAutoClosing();
    }

    private void UpdateAutoClosingTimer()
    {
        if (!m_AutoClose)
            return;

        m_AutoClosing = m_TimerBeforeClosing < 0;
        if (m_TimerBeforeClosing > 0)
            m_TimerBeforeClosing -= Time.deltaTime;
    }

    private void UpdateAutoClosing()
    {
        if (!m_AutoClosing)
            return;

        AudioManager.Instance.PlaySoundIfNotPlaying("Door_close", transform.position);
        bool done;
        if (m_Direction == Direction.Down)
        {
            m_CurrentOffset += m_ClosingSpeed * Time.deltaTime;
            done = m_CurrentOffset > 0;
        }
        else
        {
            m_CurrentOffset -= m_ClosingSpeed * Time.deltaTime;
            done = m_CurrentOffset < 0;
        }

        if (done)
        {
            m_CurrentOffset = 0;
            m_AutoClosing = false;
        }

        ApplyMovementOffset();
    }

    private void ApplyMovementOffset()
    {
        Vector3 movement = new(0, m_CurrentOffset, 0);
        transform.position = m_StartPosition + transform.TransformDirection(movement);
    }
}
