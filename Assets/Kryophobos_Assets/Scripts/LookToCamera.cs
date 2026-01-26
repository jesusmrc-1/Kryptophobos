using UnityEngine;

public class LookToCamera : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    private void Update()
    {
        FaceTextToMainCamera();
    }

    void FaceTextToMainCamera()
    {
        //Calcular la dirección en la que tiene que rotar el texto.
        Vector3 direction = transform.position - Camera.main.transform.position;
        direction.y = 0f;

        //Rotar el texto en la dirección calculada previamente
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        Quaternion smoothedRotatio = Quaternion.Slerp(transform.rotation, targetRotation, _rotateSpeed);
        transform.rotation = smoothedRotatio;
    }
}
