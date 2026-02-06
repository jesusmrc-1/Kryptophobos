using UnityEngine;

public class DestroyWalkie : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            if ( GameManager.Instance.WalkieTalkieCall != null)
            {
                Destroy(gameObject);
            }
        }
    }
}
