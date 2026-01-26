using UnityEngine;

[CreateAssetMenu(fileName = "NoteData", menuName = "Scriptable Objects/NoteData")]
public class NoteData : ScriptableObject
{
    [Tooltip("Aquí se escribe el nombre de la nota.")]
    public string NoteName;

    [Tooltip("Aquí se escribe el texto que tiene que mostrar la nota.")]
    [TextArea(1, 20)] public string NoteText;
}
