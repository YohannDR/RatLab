using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private static readonly string[] m_EnemyDeathSounds =
    {
        "Enemy_death0",
        "Enemy_death1",
        "Enemy_death2"
    };

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (!GetComponent<StatisObject>().IsFreezed() && go.TryGetComponent(out PlayerBehavior player))
        {
            player.Kill();
            return;
        }

        if (go.TryGetComponent<StatisObject>(out _))
        {
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb.velocity.sqrMagnitude > 5 * 5)
            {
                AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_EnemyDeathSounds));
                Destroy(gameObject);
            }
        }
    }
}
