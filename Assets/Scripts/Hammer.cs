using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Hammer : MonoBehaviour
{
    private static readonly string[] m_HitNoStasisSounds =
    {
        "player_attack_cheese_hammer_nostase0",
        "player_attack_cheese_hammer_nostase1",
        "player_attack_cheese_hammer_nostase2",
        "player_attack_cheese_hammer_nostase3"
    };

    private static readonly string[] m_HitStasisSounds =
    {
        "player_attack_cheese_hammer_stase0",
        "player_attack_cheese_hammer_stase1",
        "player_attack_cheese_hammer_stase2"
    };

    private BoxCollider m_Collider;

    [SerializeField] private float m_HitboxDuration;
    private float m_HitboxTimer;

    void Awake()
    {
        m_Collider = GetComponent<BoxCollider>();
        m_Collider.enabled = false;
    }

    void Update()
    {
        if (m_HitboxTimer > 0)
        {
            m_HitboxTimer -= Time.deltaTime;
            if (m_HitboxTimer < 0)
            {
                m_Collider.enabled = false;
                m_HitboxTimer = -1;
            }
        }
    }

    public void Hit()
    {
        m_Collider.enabled = true;
        m_HitboxTimer = m_HitboxDuration;
    }

    private void OnDrawGizmos()
    {
        Vector3 direction = transform.parent.localScale.x < 0.0f ? Vector3.left : Vector3.right;
        Vector3 position = transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(position, direction);
    }

    private void OnTriggerEnter(Collider collider)
    {
        m_Collider.enabled = false;

        if (collider.gameObject.TryGetComponent(out BossHand hand))
        {
            hand.TakeDamage();
            return;
        }

        if (!collider.gameObject.TryGetComponent(out StatisObject so) || !so.IsFreezed())
        {
            AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_HitNoStasisSounds));
            return;
        }

        AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_HitStasisSounds));

        Vector3 direction = transform.root.localScale.x < 0.0f ? Vector3.left : Vector3.right;
        Vector3 position = transform.position;
        // Debug.Log(position);
        bool colliding = Physics.Raycast(position, direction, out RaycastHit hit, 1.0f);

        if (!colliding)
            return;

        so.AccumulateForce(hit.point);
    }
}
