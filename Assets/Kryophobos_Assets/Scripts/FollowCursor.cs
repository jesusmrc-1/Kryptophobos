using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCursor : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;

    private void Update()
    {
        //Calcular la dirección en la que tiene que rotar la luz
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        //Aplicar rotación.
        Quaternion targetRotation = Quaternion.LookRotation(ray.direction);
        Quaternion smoothRotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        transform.rotation = smoothRotation;
    }
}
