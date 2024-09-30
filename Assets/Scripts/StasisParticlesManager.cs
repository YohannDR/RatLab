using System;
using UnityEngine;

public class StasisParticlesManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_StasingObject;
    [SerializeField] private ParticleSystem m_StasisHammerHit;
    [SerializeField] private ParticleSystem m_StasisBeam;
    [SerializeField] private ParticleSystem m_StasisBeamEnemy;
    [SerializeField] private ParticleSystem[] m_StasisHit;
    [SerializeField] private ParticleSystem[] m_StasisRelease;

    public static StasisParticlesManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void PlayStasing(Vector3 position)
    {
        ParticleSystem system = Instantiate(m_StasingObject, position, Quaternion.identity);
        system.Play();
    }

    public void PlayStasisHit(Vector3 position)
    {
        ParticleSystem system = Instantiate(m_StasisHammerHit, position, Quaternion.identity);
        system.Play();
    }

    public void PlayStasisBeam(Vector3 position, Vector2 direction)
    {
        float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        ParticleSystem system = Instantiate(m_StasisBeamEnemy, position, Quaternion.Euler(-angle, 90.0f, 0.0f));
        system.Play();
    }

    public void PlayStasisBeamEnemy(Vector3 position, Vector2 direction)
    {
        float angle = MathF.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

        ParticleSystem system = Instantiate(m_StasisBeamEnemy, position, Quaternion.Euler(-angle, 90.0f, 0.0f));
        system.Play();
    }

    public void PlayStasisHit(Vector3 position, uint force)
    {
        if (force == 0)
            return;

        ParticleSystem system = Instantiate(m_StasisHit[force - 1], position, Quaternion.identity);
        system.Play();
    }

    public void PlayStasisRelease(Vector3 position, uint force)
    {
        if (force == 0)
            return;

        ParticleSystem system = Instantiate(m_StasisRelease[force - 1], position, Quaternion.identity);
        system.Play();
    }
}
