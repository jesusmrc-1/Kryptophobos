using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Al hacer privado el setter (set), no se puede hacer desde otro script GameManager.Instace = SOMETHING, pero si acceder con el getter (get)
    //GameManager.Instance.Method()..
    public static GameManager Instance { get; private set; }
    public string LastActiveCamera = "";
    public Vector3 PlayerPosition = Vector3.zero;
    public List<ItemStack> PlayerItems { get; private set; } = new List<ItemStack>();
    public List<NoteData> PlayerNotes { get; private set; } = new List<NoteData>();
    public List<string> Objectives { get; private set; } = new List<string>();
    public List<bool> CompletedObjectives { get; private set; } = new List<bool>();
    public List<string> Doors { get; private set; } = new List<string>();
    public List<string> Puzzles { get; private set; } = new List<string>();
    public List<string> NewObjectiveTriggers { get; private set; } = new List<string>();
    public List<string> CheckpointTriggers { get; private set; } = new List<string>();
    public List<string> CutsceneTriggers { get; private set; } = new List<string>();
    public List<string> VFX { get; private set; } = new List<string>();

    public List<string> IDList { get; private set; } = new List<string>();
    public bool PlayerHasFlashlight;
    public WalkieTalkieCall WalkieTalkieCall;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "MainMenu")
        {
            ClearSavedData();
        }
    }

    public void ClearSavedData()
    {
        LastActiveCamera = "";
        PlayerPosition = Vector3.zero;
        PlayerItems.Clear();
        PlayerNotes.Clear();
        Objectives.Clear();
        CompletedObjectives.Clear();
        Doors.Clear();
        Puzzles.Clear();
        NewObjectiveTriggers.Clear();
        CheckpointTriggers.Clear();
        CutsceneTriggers.Clear();
        VFX.Clear();
        IDList.Clear();
        PlayerHasFlashlight = false;
    }
}