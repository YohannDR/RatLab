using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    private static readonly string[] m_LevelMusics =
    {
        "Level1Music",
        "Level4Music",
        "Level3Music",
        "Level4Music",
        "Level5Music"
    };

    private static readonly string[] m_LevelAmbiances =
    {
        "Amb_tuto",
        "Amb_level",
        "Amb_level",
        "Amb_level",
        "Amb_boss"
    };

    [Serializable]
    private class Sound
    {
        public string Name;
        public AudioClip Clip;
        [Range(0.0f, 2.0f)]
        public float Volume = 1.0f;
        [Range(-3.0f, 3.0f)]
        public float Pitch = 1.0f;

        public bool Loop;

        [HideInInspector]
        public AudioSource Source;
    }

    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] m_Sounds;

    private Transform m_Player;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        m_Player = FindObjectOfType<PlayerBehavior>().transform;
        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in m_Sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            s.Source = source;
            source.volume = s.Volume;
            source.pitch = s.Pitch;
            source.clip = s.Clip;
            source.loop = s.Loop;
        }

        uint level = GetCurrentLevel();
        PlaySound(m_LevelMusics[level]);
        PlaySound(m_LevelAmbiances[level]);
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(m_Sounds, s => s.Name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound named \"{name}\" does not exists");
            return;
        }
        s.Source.Play();
    }

    public void PlaySound(string name, Vector3 sourcePosition)
    {
        Vector3 srcPos = sourcePosition;
        srcPos.z = 0;
        Vector3 playerPos = m_Player.position;
        playerPos.z = 0;

        float distance = Mathf.Abs(Vector3.Distance(srcPos, playerPos));
        if (distance > 25)
            return;

        float volume = Mathf.Clamp(1 - distance / 25, 0, 1);

        Sound s = Array.Find(m_Sounds, s => s.Name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound named \"{name}\" does not exists");
            return;
        }

        AudioSource source = s.Source;
        source.volume = volume;
        source.Play();
    }

    public void PlaySoundIfNotPlaying(string name, Vector3 sourcePosition)
    {
        Vector3 srcPos = sourcePosition;
        srcPos.z = 0;
        Vector3 playerPos = m_Player.position;
        playerPos.z = 0;

        float distance = Mathf.Abs(Vector3.Distance(srcPos, playerPos) - 5);
        if (distance > 25)
            return;

        float volume = Mathf.Clamp(1 - distance / 25, 0, 1);

        Sound s = Array.Find(m_Sounds, s => s.Name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound named \"{name}\" does not exists");
            return;
        }

        AudioSource source = s.Source;
        source.volume = volume;

        if (source.isPlaying)
            return;

        source.Play();
    }

    public void PlaySoundIfNotPlaying(string name)
    {
        Sound s = Array.Find(m_Sounds, s => s.Name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound named \"{name}\" does not exists");
            return;
        }
        AudioSource source = s.Source;

        if (source.isPlaying)
            return;

        source.Play();
    }

    public void StopSound(string name)
    {
        Sound s = Array.Find(m_Sounds, s => s.Name == name);
        if (s == null)
        {
            Debug.LogWarning($"Sound named \"{name}\" does not exists");
            return;
        }
        s.Source.Stop();
    }

    public static string GetRandomSound(string[] list) => list[Random.Range(0, list.Length)];

    public static uint GetCurrentLevel()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Level 01 - Tutorial":
                return 0;

            case "Level 02":
                return 1;

            case "Level 03":
                return 2;

            case "Level 04":
                return 3;

            case "Boss_Scene":
                return 4;
        }

        return 0;
    }
}
