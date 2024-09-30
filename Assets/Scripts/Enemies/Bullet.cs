using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    private Vector3 m_Direction;

    public void Setup(Vector3 direction)
    {
        m_Direction = direction;
        transform.GetChild(0).rotation = Quaternion.LookRotation(direction, Vector3.forward);
    }

    void Update()
    {
        transform.Translate(m_Direction * (m_Speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider collision)
    {
        GameObject go = collision.gameObject;
        if (go.TryGetComponent(out PlayerBehavior player))
        {
            player.Kill();
            return;
        }

        Destroy(gameObject);
    }
}
