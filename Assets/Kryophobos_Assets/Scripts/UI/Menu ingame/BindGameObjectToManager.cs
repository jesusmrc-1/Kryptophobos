using UnityEngine;

public class BindGameObjectToManager : MonoBehaviour
{
    [SerializeField] private bool dontDisableThisGameObject;
    void Awake()
    {
        UIManager.Instance.SaveRefsFromMenuIngame(this.gameObject);
        Debug.Log(this.gameObject.name);

        if (!dontDisableThisGameObject) this.gameObject.SetActive(false);
    }
}
