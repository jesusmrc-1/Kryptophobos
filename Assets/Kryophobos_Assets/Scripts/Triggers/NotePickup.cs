using UnityEngine;

public class NotePickup : TriggerBase
{
    [SerializeField] private NoteData _noteData;

    private Player _player;
    private PlayerInventory _playerInventory;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();

        LoadStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _player.IsPlayerNearNote = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        _player.IsPlayerNearNote = false;
    }

    //Coger la nota.
    public void PickupNote()
    {
        if (_noteData != null) _playerInventory.Notes.Add(_noteData);
        else if (_noteData == null) Debug.LogError("ERROR: El trigger no tiene referenciada la nota (Scriptable Object).");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerNotes.Add(_noteData);
        }

        GameObject newUIManagerGO = GameObject.FindGameObjectWithTag("UIManager");
        if (newUIManagerGO != null)
        {
            NewUIManager newUIManager = newUIManagerGO.GetComponent<NewUIManager>();

            if (newUIManager != null)
            {
                newUIManager.OpenIngameMenu("Notes");

            }
        }

        Destroy(gameObject);
    }

    private void LoadStatus()
    {
        NoteData noteData = GameManager.Instance.PlayerNotes.Find(NoteData => NoteData == _noteData);

        if (noteData == _noteData)
        {
            Destroy(gameObject);
        }
    }
}
