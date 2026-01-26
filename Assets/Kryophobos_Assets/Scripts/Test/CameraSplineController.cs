using Unity.Cinemachine;
using UnityEngine;

public class CameraSplineController : MonoBehaviour
{
    public CinemachineCamera vcam;
    public float speed = 0.2f;

    private CinemachineSplineDolly dolly;

    private float _startingCameraPos;

    public float CameraPos;

    [SerializeField] private bool _swapCamera;

    [SerializeField] private GameObject _nextCamera;

    private CinemachineRotationComposer _composer;

    void Start()
    {
        dolly = vcam.GetComponent<CinemachineSplineDolly>();
        _composer = GetComponent<CinemachineRotationComposer>();

        _startingCameraPos = dolly.CameraPosition;
    }

    void Update()
    {
        if (dolly == null) return;

        CameraPos = dolly.CameraPosition;

        dolly.CameraPosition += speed * Time.deltaTime;
        dolly.CameraPosition = Mathf.Clamp01(dolly.CameraPosition);

        if (dolly.CameraPosition >= 0.7)
        {
            //Cuando este en X porcentaje del spline, volver a mirar al jugador.
            //vcam.LookAt = GameObject.FindGameObjectWithTag("Player").transform;
            //vcam.enabled = true;
            //_composer.enabled = true;
            _composer.TargetOffset.y = 1f;
            //HACER QUE CUANDO HAYA CAMBIO DE CAMARA, EN X CAMARAS DEJE DE MIRAR AL JUGADOR
        }

        if (dolly.CameraPosition == 1 && _swapCamera)
        {
            if (_nextCamera != null) _nextCamera.SetActive(true);
            this.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        dolly.CameraPosition = _startingCameraPos;
        //vcam.LookAt = null;
        //vcam.enabled = false;
        //_composer.enabled = false;
        _composer.TargetOffset.y = 2.37f;
    }
}