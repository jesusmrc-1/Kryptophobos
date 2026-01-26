using Unity.Cinemachine;
using UnityEngine;

public class SwapCinemachineCameras : MonoBehaviour
{
    private CameraManager _cameraManager;
    private enum angles { Fixed_Camera,N_0º, NE_45º, E_90º, SE_135º, S_180º, SO_225º, O_270º, NO_315º }
    [Tooltip("Puedes elegir un angulo de camara que pertenece a las camaras que sigue el jugador, o una camara fija del mundo (Fixed Camera).")]
    [SerializeField] private angles CameraAngleA;
    [Tooltip("Puedes elegir un angulo de camara que pertenece a las camaras que sigue el jugador, o una camara fija del mundo (Fixed Camera).")]
    [SerializeField] private angles CameraAngleB;

    [Tooltip("Aquí va la referencia de la camara que se activa si el jugador esta mas cerca de 'A'")]
    [SerializeField] private GameObject CameraA;

    [Tooltip("Aquí va la referencia de la camara que se activa si el jugador esta mas cerca de 'B'")]
    [SerializeField] private GameObject CameraB;

    [Tooltip("El tiempo que tarda en cambiar de otra camara, a la camara 'A'.")]
    [SerializeField] private float _cameraABlendTime;
    [Tooltip("El tiempo que tarda en cambiar de otra camara, a la camara 'B'.")]
    [SerializeField] private float _cameraBBlendTime;

    [Tooltip("Aquí va la referencia del punto 'A'.")]
    [SerializeField] private GameObject a;
    [Tooltip("Aquí va la referencia del punto 'B'.")]
    [SerializeField] private GameObject b;

    #region Get Cameras
    private void Start()
    {
        _cameraManager = GameObject.FindGameObjectWithTag("CameraManager").GetComponent<CameraManager>();

        //Dependiendo de que angulo de camara este configurado en el inspector, se busca la referencia de la Cinemachine Camera en '_cameraManager.CameraList'
        //y nos la devuelve a 'CameraA'.
        CameraA = GetCameraFromAngle(CameraAngleA,CameraA);

        //Dependiendo de que angulo de camara este configurado en el inspector, se busca la referencia de la Cinemachine Camera en '_cameraManager.CameraList'
        //y nos la devuelve a 'CameraB'.
        CameraB = GetCameraFromAngle(CameraAngleB, CameraB);
    }

    //GetCameraFromAngle -> GetCameraFromAngle -> SearchCamera -> GetCameraFromAngler -> GetCameraFromAngle.
    GameObject GetCameraFromAngle(angles CameraAngleX, GameObject cameraX)
    {

        switch (CameraAngleX)
        {
            case angles.Fixed_Camera:
                return cameraX; //Si la opcion es Fixed Camera, devolvemos la propia camara que es la que esta referenciada de forma manual, no perdemos la referencia.
            case angles.N_0º:
                return SearchCamera("CM_Camera_N_0º");
            case angles.NE_45º:
                return SearchCamera("CM_Camera_NE_45º");
            case angles.E_90º:
                return SearchCamera("CM_Camera_E_90º");
            case angles.SE_135º:
                return SearchCamera("CM_Camera_SE_135º");
            case angles.S_180º:
                return SearchCamera("CM_Camera_S_180º");
            case angles.SO_225º:
                return SearchCamera("CM_Camera_SO_225º");
            case angles.O_270º:
                return SearchCamera("CM_Camera_O_270º");
            case angles.NO_315º:
                return SearchCamera("CM_Camera_NO_315º");
        }
        return null;
    }
    GameObject SearchCamera(string cameraName)
    {
        foreach (GameObject camera in _cameraManager.CameraList)
        {
            if (camera.name == cameraName)
            {
                return camera;
            }
        }
        return null;
    }
    #endregion

    private void OnTriggerStay(Collider other)
    {
        if (other.tag != "Player") return;

        //Comprobar si el jugador esta mas cerca de 'A' o de 'B'

        Vector3 playerPosition = other.transform.position;
        Vector3 a = this.a.transform.position;
        Vector3 b = this.b.transform.position;


        float distanceFromA = Vector3.Distance(playerPosition, a);
        float distanceFromB = Vector3.Distance(playerPosition, b);

        //El jugador esta mas cerca de A, cambiamos la camara a la posicion A
        if (distanceFromA < distanceFromB)
        {
            //El cambio de camaras lo hace _cameraManager
            _cameraManager.ChangeCameraAngle(CameraA, CameraB);
            _cameraManager.ChangeCameraBlendTime(_cameraABlendTime);
        }
        //El jugador esta mas cerca de B, cambiamos la camara a la posicion A
        else if (distanceFromB < distanceFromA)
        {
            _cameraManager.ChangeCameraAngle(CameraB, CameraA);
            _cameraManager.ChangeCameraBlendTime(_cameraBBlendTime);
        }
    }
}
