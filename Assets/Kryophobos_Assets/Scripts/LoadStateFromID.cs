using UnityEngine;

public class LoadStateFromID : MonoBehaviour
{
    [SerializeField] private string _ID;

    private void Start()
    {
        string ID = GameManager.Instance.VFX.Find(vfx => vfx == _ID);

        if (ID == _ID)
        {
            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.VFX.Add(_ID);
        }   
    }
}
