using UnityEngine;

public abstract class TriggerBase : MonoBehaviour
{
    [Tooltip("El mensaje que se muestra en los objetos, puertas y puzzles. Variable heredada de la clase TriggerBase")]
    public string DiegeticText;

    [Tooltip("El tamaño de la fuente de las letras del mensaje. Variable heredada de la clase TriggerBase")]
    public float DiegeticFontSize;

    [Tooltip("El punto exacto en el que aparece el mensaje, se usa un Empty como pivote. Variable heredada de la clase TriggerBase")]
    public GameObject DiegeticTextPosition;
}
