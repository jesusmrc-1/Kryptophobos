using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ChangeNoteText : MonoBehaviour
{
    public TextMeshProUGUI TMP;
    public string Text;

    public void OnClickChangeNoteText()
    {
        TMP.text = Text;
    }
}
