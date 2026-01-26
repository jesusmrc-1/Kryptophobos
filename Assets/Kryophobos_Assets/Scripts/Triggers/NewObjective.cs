using UnityEngine;

public class NewObjective : MonoBehaviour
{
    [SerializeField] private string _ID;

    [Tooltip("Aquí va el texto que mostrara el nuevo objetivo.")]
    [SerializeField][TextArea(1, 3)] private string _newObjectiveText;

    [SerializeField] private NewUIManager _newUIManager;
    private bool _isObjectiveSaved = true;

    private void Start()
    {
        _newUIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<NewUIManager>();

        LoadStatus();
    }

    public void ChangeObjective()
    {
        _newUIManager.ChangeObjective(_newObjectiveText);
        GameManager.Instance.NewObjectiveTriggers.Add(_ID);

        FocusPOI focusPOI = GetComponent<FocusPOI>();

        if (focusPOI == null) Destroy(gameObject);
    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) ChangeObjective();
    }
    */

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !_isObjectiveSaved) ChangeObjective();
    }

    private void LoadStatus()
    {
        string _ID = GameManager.Instance.NewObjectiveTriggers.Find(_ID => _ID == this._ID);

        if (_ID == this._ID)
        {
            Destroy(gameObject);
        }
        else
        {
            _isObjectiveSaved = false;
        }
    }
}
