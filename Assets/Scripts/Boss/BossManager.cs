using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BossManager : MonoBehaviour
{
    private bool m_HasHandDestroyed;
    private bool m_IsDead;

    private List<BossHand> m_Hands;

    void Awake()
    {
        m_Hands = new();
        m_Hands.AddRange(FindObjectsOfType<BossHand>());
        Assert.IsTrue(m_Hands.Count == 2, "Too many hands");
    }

    void Update()
    {
        if (m_IsDead && transform.rotation.eulerAngles.x > 60)
        {
            transform.Rotate(1, 0, 0);
        }
    }

    public void RegisterHandKilled(BossHand hand)
    {
        m_Hands.Remove(hand);
        m_HasHandDestroyed = true;

        if (m_Hands.Count == 0)
        {
            m_IsDead = true;
        }
    }

    public bool ShouldAttack() => m_HasHandDestroyed;
}
