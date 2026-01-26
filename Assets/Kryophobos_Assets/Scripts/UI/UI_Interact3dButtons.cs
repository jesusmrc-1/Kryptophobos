using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class UI_Interact3dButtons : MonoBehaviour
{
    private CameraManager _cameraManager;
    private PlayVideo _playVideo;
    [SerializeField] float rotationSpeed;
    [SerializeField] InputAction interact;
    [SerializeField] private VideoClip _clip;
    [SerializeField] private string _sceneNameToLoad;

    [SerializeField] private GameObject _mainMenuCamera;
    [SerializeField] private GameObject _creditsCamera;
    [SerializeField] private float _blendTime;

    private void Awake()
    {
        interact.Enable();
        GameObject videoPlayerGO = GameObject.FindGameObjectWithTag("VideoPlayer");
        if (videoPlayerGO != null) _playVideo = videoPlayerGO.GetComponent<PlayVideo>();

        GameObject cameraManagerGO = GameObject.FindGameObjectWithTag("CameraManager");
        if (cameraManagerGO != null) _cameraManager = cameraManagerGO.GetComponent<CameraManager>();
    }

    void Update()
    {
        //Calcular la dirección en la que tiene que rotar la luz
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Aplicar rotación.
        Quaternion targetRotation = Quaternion.LookRotation(ray.direction);
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = smoothRotation;

        bool hasRayImpacted = Physics.Raycast(ray, out RaycastHit hitInfo);

        if (interact.triggered && hasRayImpacted) CheckPressedButton(hitInfo);
    }

    void CheckPressedButton(RaycastHit hitInfo)
    {
        switch (hitInfo.collider.tag)
        {
            case "UI_Start":
                GameObject VideoPlayerGO = GameObject.FindGameObjectWithTag("VideoPlayer");

                if (VideoPlayerGO != null)
                {
                    _playVideo = VideoPlayerGO.GetComponent<PlayVideo>();
                }

                if (_playVideo != null && _clip != null) _playVideo.PlayVideoAndLoadScene(_clip, _sceneNameToLoad, true);
                Destroy(gameObject);
                break;

            case "UI_Load":
                //Cargar partida
                break;

            case "UI_Options":
                //Ir a opciones (CREDITOS)
                Debug.Log("CREDITOS");
                _cameraManager.ChangeCameraBlendTime(_blendTime);
                _cameraManager.ChangeCameraAngle(_creditsCamera, _mainMenuCamera);
                break;

            case "UI_BackToMainMenu":
                //Volver al menu principal
                Debug.Log("VOLVER");
                _cameraManager.ChangeCameraBlendTime(_blendTime);
                _cameraManager.ChangeCameraAngle(_mainMenuCamera, _creditsCamera);
                break;

            case "UI_Exit":
                Application.Quit();
                break;
        }
    }
}
