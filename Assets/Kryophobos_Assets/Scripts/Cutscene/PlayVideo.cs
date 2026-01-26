using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    private NewUIManager _newUIManager;
    private EventsManager _eventsManager;
    private VideoPlayer _videoPlayer;
    private string _sceneName;
    private bool _loadScene;
    private bool _playingVideoClip;
    [SerializeField] private InputAction _skipCutscene;
    [SerializeField] private RawImage _rawImage;
    [SerializeField] private bool _showNewObjective;

    private void Awake()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        GameObject gameEventsGO = GameObject.FindGameObjectWithTag("GameEventsManager");
        if (gameEventsGO != null) _eventsManager = gameEventsGO.GetComponent<EventsManager>();
        _rawImage.enabled = false;

        GameObject newUIManagerGO = GameObject.FindGameObjectWithTag("UIManager");
        if (newUIManagerGO != null) _newUIManager = newUIManagerGO.GetComponent<NewUIManager>();
    }

    private void Update()
    {
        if (_skipCutscene.triggered && _playingVideoClip) OnVideoEnd(_videoPlayer);
    }

    //Reproducir video y proceder al cambio de escena mas adelante
    public void PlayVideoAndLoadScene(VideoClip clip, string sceneName, bool loadScene)
    {
        if (_newUIManager != null) _newUIManager.IngameMenu.Disable();
        if (_rawImage != null) _rawImage.enabled = true;
        _skipCutscene.Enable();
        _playingVideoClip = true;
        if (_eventsManager != null) _eventsManager.PauseGame();
        _sceneName = sceneName;
        _loadScene = loadScene;
        PlayVideoClip(clip);
    }

    //Reproducir video
    public void PlayVideoClip(VideoClip clip)
    {
        if (_newUIManager != null) _newUIManager.IngameMenu.Disable();
        if (_rawImage != null) _rawImage.enabled = true;
        _skipCutscene.Enable();
        _playingVideoClip = true;
        if (_eventsManager != null) _eventsManager.PauseGame();

        //Nos suscribimos al evento interno 'loopPointReached' y llamamos a nuestra función.
        _videoPlayer.loopPointReached += OnVideoEnd;

        _videoPlayer.clip = clip;

        _videoPlayer.Play();
    }

    //Se termina el video y se lee nuevo codigo
    private void OnVideoEnd(VideoPlayer vp)
    {
        if (_eventsManager != null) _eventsManager.ResumeGame();
        _videoPlayer.Stop();
        if (_rawImage != null) _rawImage.enabled = false;
        _skipCutscene.Disable();
        _playingVideoClip = false;

        //Si al terminar se quiso cambiar de escena, entonces se llama a ManageScene de EventsManager
        if (_loadScene)
        {
            Debug.Log("OnViedoEnd " + _loadScene);
            if (_eventsManager != null) StartCoroutine(_eventsManager.ManageScene(_sceneName));
        }

        if (_showNewObjective)
        {
            NewObjective newObjective = GetComponent<NewObjective>();
            
            if (newObjective != null)
            {
                newObjective.ChangeObjective();
            }
        }
        if (_newUIManager != null) _newUIManager.IngameMenu.Enable();
    }
}
