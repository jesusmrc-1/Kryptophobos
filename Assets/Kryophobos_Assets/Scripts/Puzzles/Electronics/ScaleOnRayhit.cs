using UnityEngine;

public class ScaleOnRayhit : MonoBehaviour
{
    public Vector3 Scale;
    public bool ScaleObject;
    private void Update()
    {
        if (Scale != null && ScaleObject) transform.localScale = Scale;
    }

    public void ResetScale()
    {
        transform.localScale = Vector3.one;
    }
}
