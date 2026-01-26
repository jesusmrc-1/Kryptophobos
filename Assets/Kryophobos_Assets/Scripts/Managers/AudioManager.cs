using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Buses / Categories")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource sfx2DSource;

    [Header("3D SFX Pool")]
    [SerializeField] private int poolSize = 15;
    private List<AudioSource> sfx3DSources;

    [Header("Global Volumes")]
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;
    [Range(0, 1)] public float uiVolume = 1f;
    [Range(0, 1)] public float ambienceVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreateSFX3DPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        musicSource.volume = musicVolume;
        uiSource.volume = uiVolume;
        ambienceSource.volume = ambienceVolume;
        sfx2DSource.volume = sfxVolume;
    }

    private void CreateSFX3DPool()
    {
        sfx3DSources = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject go = new GameObject("SFX3D Source " + i);
            go.transform.parent = transform;

            AudioSource src = go.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.spatialBlend = 1f; // 3D
            src.rolloffMode = AudioRolloffMode.Linear;

            sfx3DSources.Add(src);
        }
    }

    private AudioSource GetFree3DSource()
    {
        foreach (var src in sfx3DSources)
        {
            if (!src.isPlaying)
                return src;
        }

        return sfx3DSources[0]; // fallback
    }

    // ------------ PUBLIC API ------------ //

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void PlayUISound(AudioClip clip)
    {
        uiSource.PlayOneShot(clip, uiVolume);
    }

    public void Play2DSFX(AudioClip clip)
    {
        sfx2DSource.PlayOneShot(clip, sfxVolume);
    }

    public void Play3DSFX(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        Debug.Log(this + " Play3dSFX");

        AudioSource src = GetFree3DSource();

        src.transform.position = position;
        src.pitch = pitch;
        src.volume = volume * sfxVolume;
        src.PlayOneShot(clip);
    }

    public void PlayFootstep(FootstepSet set, Vector3 position)
    {
        Debug.Log(this + " PlayFootstep");
        if (set == null) return;

        AudioClip clip = set.GetRandomClip();
        if (clip == null) return;

        Play3DSFX(
            clip,
            position,
            set.volume,
            set.GetRandomPitch()
        );
    }
}