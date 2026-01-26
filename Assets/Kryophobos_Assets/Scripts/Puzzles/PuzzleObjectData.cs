using UnityEngine;

public class PuzzleObjectData : MonoBehaviour
{
    //Optional depending on puzzle
    public GameObject PrefabToShow;
    public GameObject PrefabToInstantiate;

    //This ID will be saved in puzzles when putting objects on them
    public string ID;

    //This will let us check to recover or not the clicked object
    public bool IsObjectPlaced;

    public GameObject SelectableObject;

    public int SocketIndex;
}
