using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(AudioSource))]
public class Cheese : MonoBehaviour
{
    [SerializeField] private float m_HoverMultiplier;
    [SerializeField] private float m_HoverSpeed;

    private Vector3 m_StartingPosition;

    private void Awake()
    {
        m_StartingPosition = transform.position;
    }

    private void Update()
    {
        Vector3 offset = new(0, Mathf.Sin(Time.realtimeSinceStartup * m_HoverSpeed) * m_HoverMultiplier, 0);
        transform.position = m_StartingPosition + offset;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        CheeseManager.RegisterCheese();
        AudioManager.Instance.PlaySound("Pickup_collectible");

        // TODO particles
        Destroy(gameObject);
    }
}
