using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Tooltip("Aquí se referencia la Cinemachine Camera en la que queremos que empiece activa al principio del nivel.")]
    public GameObject StartingCamera;

    [HideInInspector]
    public GameObject[] CameraList;

    private GameObject _activeCamera;

    private GameObject _mainCamera;

    private void Awake()
    {
        //Guardamos las referencias de las Cinemachine Cameras que siguen al jugador.
        CameraList = GameObject.FindGameObjectsWithTag("CM_Camera");

        _mainCamera = Camera.main.gameObject;
    }

    private void Start()
    {
        LoadState();

        //Desactivamos las camaras y activamos la camara configurada en el inspector para ser activa al principio del nivel.
        DisableCams();
    }

    // ...
    void DisableCams()
    {
        foreach (GameObject camera in CameraList) camera.SetActive(false);
        StartingCamera.SetActive(true);
    }

    //Orden de llamada: ChangeCameraAngle.cs -> CameraManager.cs
    //Activamos la Cinemachine Camera correspondiente (cameraToEnable) / Desactivamos la Cinemachine Camera activa (cameraToDisable).
    public void ChangeCameraAngle(GameObject cameraToEnable, GameObject cameraToDisable)
    {
        foreach (GameObject camera in CameraList)
        {
            if (camera == cameraToDisable) camera.SetActive(false);
            if (camera == cameraToEnable) 
            {
                camera.SetActive(true);
                _activeCamera = camera;
            }
        }
    }

    //Cambiamos el timepo que tarda en el cambio de una camara a otra.
    public void ChangeCameraBlendTime(float blendTime)
    {
        CinemachineBrain cinemachineBrain = _mainCamera.GetComponent<CinemachineBrain>();
        cinemachineBrain.DefaultBlend.Time = blendTime;
    }

    #region Puzzle Cameras
    //Enfocamos al puzle.
    public void FocusPuzzle(GameObject puzzleCamera)
    {
        _activeCamera.SetActive(false);
        puzzleCamera.SetActive(true);
    }

    //Dejamos de enfocar al puzle.
    public void UnfocusPuzzle(GameObject puzzleCamera)
    {
        puzzleCamera.SetActive(false);
        _activeCamera.SetActive(true);
    }
    #endregion

    private void LoadState()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.LastActiveCamera != "")
        {
            if (StartingCamera != null) StartingCamera = GameObject.Find(GameManager.Instance.LastActiveCamera);
        }
    }
}