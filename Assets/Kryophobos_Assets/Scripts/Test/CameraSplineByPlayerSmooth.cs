using System.Drawing;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSplineByPlayerSmooth : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera vcam;
    public Transform player;

    [Header("Player mapping (world space)")]
    public float playerMinAxisValue;
    public float playerMaxAxisValue;

    [Header("Smoothing")]
    public float smoothSpeed = 3f;

    private CinemachineSplineDolly dolly;

    [Header("Axis")]
    [SerializeField] private bool _x;
    [SerializeField] private bool _y;
    [SerializeField] private bool _z;

    void Start()
    {
        dolly = vcam.GetComponent<CinemachineSplineDolly>();
    }

    void Update()
    {
        if (dolly == null || player == null)
            return;

        // Convertimos la posición del jugador a un valor normalizado (0..1)
        float targetPosition = 0;

        if (_x)
        {
            targetPosition = Mathf.InverseLerp(playerMinAxisValue, playerMaxAxisValue, player.position.x);
        }
        else if (_y)
        {
            targetPosition = Mathf.InverseLerp(playerMinAxisValue, playerMaxAxisValue, player.position.y);
        }
        else if (_z)
        {
            targetPosition = Mathf.InverseLerp(playerMinAxisValue, playerMaxAxisValue, player.position.z);
        }

        // Suavizamos el movimiento de la cámara en el spline
        dolly.CameraPosition =
            Mathf.Lerp(dolly.CameraPosition, targetPosition, Time.deltaTime * smoothSpeed);
    }
}