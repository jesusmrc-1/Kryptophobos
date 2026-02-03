using UnityEngine;

public class LoadStateFromIDRemoval : MonoBehaviour
{
    [SerializeField] private string _ID;

    private void Start()
    {
        string ID = GameManager.Instance.IDList.Find(id => id == _ID);

        if (ID == _ID)
        {
            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.IDList.Add(_ID);
        }   
    }

    public string GetID()
    {
        return _ID;
    }
}
