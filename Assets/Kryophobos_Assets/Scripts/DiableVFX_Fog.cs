using UnityEngine;

public class DiableVFX_Fog : MonoBehaviour
{
    [SerializeField] private GameObject[] ToDisable;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            foreach (GameObject obj in ToDisable)
            {
                LoadStateFromIDRemoval loadStateFromIDRemoval = obj.GetComponent<LoadStateFromIDRemoval>();
                if (loadStateFromIDRemoval != null)
                {
                    string ID = loadStateFromIDRemoval.GetID();
                    GameManager.Instance.IDList.Add(ID);
                }
                obj.SetActive(false);
            }
        }
    }
}
