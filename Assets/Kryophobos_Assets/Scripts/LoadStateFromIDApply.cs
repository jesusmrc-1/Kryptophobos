using UnityEngine;

public class LoadStateFromIDApply : MonoBehaviour
{
    [SerializeField] private string _ID;
    public BoxCollider _collider;
    MeshRenderer _meshRenderer;
    public GameObject[] BlockingAntennaParts;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<BoxCollider>();
    }
    private void OnEnable()
    {
        string ID = GameManager.Instance.IDList.Find(id => id == _ID);

        if (ID == _ID)
        {
            gameObject.SetActive(true);

            if (_meshRenderer != null) _meshRenderer.enabled = true;
            if (_collider != null) _collider.enabled = true;
            foreach (var obj in BlockingAntennaParts)
            {
                if (obj != null)
                {
                    MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                    if (meshRenderer != null)
                    {
                        meshRenderer.enabled = true;
                    }
                }
            }
        }
        else
        {
            GameManager.Instance.IDList.Add(_ID);
        }   
    }
}
