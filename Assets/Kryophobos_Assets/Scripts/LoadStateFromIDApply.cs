using UnityEngine;

public class LoadStateFromIDApply : MonoBehaviour
{
    [SerializeField] private string _ID;

    private void OnEnable()
    {
        string ID = GameManager.Instance.IDList.Find(id => id == _ID);

        if (ID == _ID)
        {
            gameObject.SetActive(true);
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
