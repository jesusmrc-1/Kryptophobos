using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class AmbienceManager : MonoBehaviour
{
    public enum CurrentScene { MainMenu, Level1 }
    [SerializeField] private CurrentScene scene;

    [SerializeField] private AudioClip[] clips;

    private Dictionary<string, AudioClip> clipList;

    [SerializeField] private AudioSource mainMenu;
    [SerializeField] private AudioSource gameplay;
    [SerializeField] private AudioSource gameplayTension;
    [SerializeField] private AudioSource gameplayPersecution;

    public List<Enemy> enemiesSearching;
    public List<Enemy> enemiesFollowing;

    private void Awake()
    {
        clipList = new Dictionary<string, AudioClip>();

        foreach (var clip in clips)
        {
            if (!clipList.ContainsKey(clip.name))
                clipList.Add(clip.name, clip);
        }

        enemiesSearching = new List<Enemy>();
        enemiesFollowing = new List<Enemy>();
    }

    private void Start()
    {
        switch (scene)
        {
            case CurrentScene.MainMenu:
                StartCoroutine(PlayIntroThenLoop(mainMenu, "Start_intro", "Start_loop"));
                break;

            case CurrentScene.Level1:
                StartCoroutine(PlayIntroThenLoop(gameplay, "Gameplay_intro", "Gameplay_loop"));
                break;
        }
    }

    private IEnumerator PlayIntroThenLoop(AudioSource source, string intro, string loop)
    {
        if (!clipList.TryGetValue(intro, out AudioClip introClip))
            yield break;

        source.loop = false;
        source.clip = introClip;
        source.Play();

        yield return new WaitWhile(() => source.isPlaying);

        PlayLoop(source, loop);
    }

    private void PlayLoop(AudioSource source, string clipName)
    {
        if (clipList.TryGetValue(clipName, out AudioClip clip))
        {
            source.clip = clip;
            source.loop = true;
            source.Play();
        }
    }

    //Enemigo en modo busqueda, el enemigo se añade a la lista de 'enemiesSearching' y luego llama al metodo, si ya hay algun enemigo, entonces no se vuelve a reproducir
    public void TensionSFX(bool playSFX)
    {
        if(enemiesSearching.Count == 0)
        {
            if (gameplayTension != null)
            {
                if (playSFX) gameplayTension.Play();
                else gameplayTension.Stop();
            }
        }
    }

    //Enemigo persiguiendo al jugador
    public void PersecutionSFX(bool playSFX)
    {
        if (enemiesFollowing.Count == 0)
        {
            if (gameplayPersecution != null)
            {
                if (playSFX) gameplayPersecution.Play();
                else gameplayPersecution.Stop();
            }
        }
    }
}
