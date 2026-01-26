using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 150f;
    [SerializeField] private bool _rotateX;
    [SerializeField] private bool _rotateY;
    [SerializeField] private bool _rotateZ;

    void Update()
    {
        if (_rotateX)
        {
            transform.Rotate(_rotationSpeed * Time.deltaTime, 0 , 0);
        }
        if (_rotateY)
        {
            transform.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
        }
        if (_rotateZ)
        {
            transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
        }
    }
}
