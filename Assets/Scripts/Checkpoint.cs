using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool m_DestroyOnCollision;

    public Vector3 SpawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        if (go.CompareTag("Player"))
        {
            CheckpointManager.RegisterLastCheckpoint(this);
            return;
        }

        if (m_DestroyOnCollision)
            Destroy(go);
    }
}
