using UnityEngine;
using UnityEngine.SceneManagement;

public static class CheckpointManager
{
    public static Vector3 LastCheckpointPosition;
    public static byte HealthBackup;

    public static void RegisterLastCheckpoint(Checkpoint checkpoint) => LastCheckpointPosition = checkpoint.SpawnPosition;

    public static void LoadLastCheckpoint(byte health)
    {
        HealthBackup = health;
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }
}
