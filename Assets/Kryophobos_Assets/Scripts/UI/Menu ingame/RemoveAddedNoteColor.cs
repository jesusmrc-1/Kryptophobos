using UnityEngine;
using UnityEngine.UI;

public class RemoveAddedNoteColor : MonoBehaviour
{
    private Button _button;
    private bool _removedNotification;
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        //Comprobar el inventario del jugador, si ya existe la nota que corresponde a este boton, entonces quitar el color amarillo.
    }

    public void RemoveColorNotification()
    {
        if (_button != null && !_removedNotification)
        {
            ColorBlock colorBlock = _button.colors;

            Color alpha = Color.black;
            alpha.a = 0;

            colorBlock.normalColor = alpha;
            _button.colors = colorBlock;

            _removedNotification = true;
        }
    }
}
