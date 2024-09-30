using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject m_ObjectToSpawn;
    [SerializeField] private float m_SpawnInterval;
    [SerializeField] private bool m_SpawnContinuously;

    private float m_SpawnTimer;
    private GameObject m_SpawnedObjectBuffer;

    void Start()
    {
        Assert.IsTrue(m_ObjectToSpawn != null);

        m_SpawnTimer = m_SpawnInterval;
    }

    void Update()
    {
        if (!CheckObjectDespawned())
            return;

        m_SpawnTimer -= Time.deltaTime;
        if (m_SpawnTimer > 0)
            return;

        m_SpawnTimer = m_SpawnInterval;
        GameObject go = Instantiate(m_ObjectToSpawn, transform.position, Quaternion.identity);

        if (m_SpawnContinuously)
            return;

        m_SpawnedObjectBuffer = go;
    }

    private bool CheckObjectDespawned()
    {
        if (m_SpawnedObjectBuffer == null)
            return true;

        return !m_SpawnedObjectBuffer.activeSelf;
    }
}
