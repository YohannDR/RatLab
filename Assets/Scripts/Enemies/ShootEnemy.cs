using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemy : Enemy
{
    private static readonly string[] m_EnemyShootNormalSounds =
    {
        "ennemy_shoot_normal0",
        "ennemy_shoot_normal1",
        "ennemy_shoot_normal2"
    };

    [SerializeField] private GameObject m_Bullet;
    [SerializeField] private GameObject m_CannonTip;
    [SerializeField] private float m_DelayAfterShoot;

    private ConeVisionBehavior m_ConeVisionBehavior;
    private GameObject m_Player;

    private StatisObject m_StatisObject;

    void Awake()
    {
        m_ConeVisionBehavior = GetComponent<ConeVisionBehavior>();
        SetupStatisObject();
    }

    private void SetupStatisObject()
    {
        if (!TryGetComponent(out m_StatisObject))
            return;

        m_StatisObject.OnFreezeCallbacks += () => { if (m_Player == null) m_ConeVisionBehavior.Disable(); };
        m_StatisObject.OnUnfreezeCallbacks += () => { if (m_Player == null) m_ConeVisionBehavior.Enable(); };
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (!go.CompareTag("Player"))
            return;

        AudioManager.Instance.PlaySound("Ennemy_detect_player");
        m_Player = go;
        StartCoroutine(ShootInterval());

        AudioManager.Instance.PlaySound(AudioManager.GetRandomSound(m_EnemyShootNormalSounds), transform.position);

        Vector3 position = m_CannonTip.transform.position;
        Vector3 direction = m_Player.transform.position - position;
        GameObject bullet = Instantiate(m_Bullet, position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Setup(direction);
    }

    private IEnumerator ShootInterval()
    {
        m_ConeVisionBehavior.Disable();
        yield return new WaitForSeconds(m_DelayAfterShoot);
        m_ConeVisionBehavior.Enable();
    }
}
