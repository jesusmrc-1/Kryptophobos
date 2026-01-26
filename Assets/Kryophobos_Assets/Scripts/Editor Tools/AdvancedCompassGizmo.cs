using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class AdvancedCompassGizmo : MonoBehaviour
{
    [Header("Ajustes de Brújula")]
    public float radius = 3f;
    public float labelSize = 16f;
    public Color mainColor = Color.white;
    public Color secondaryColor = Color.gray;
    public Color degreeColor = Color.yellow;

    [Header("Mostrar grados")]
    public bool showDegrees = true;
    public int degreeStep = 45; // Cada cuántos grados mostrar (45 = 8 puntos cardinales)

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 pos = transform.position;
        Handles.color = mainColor;

        // Dibuja un círculo representando la brújula
        Handles.DrawWireDisc(pos, Vector3.up, radius);

        // Calcular direcciones cardinales
        string[] directions = new string[]
        {
            "N", "NE", "E", "SE", "S", "SO", "O", "NO"
        };

        // Dibuja etiquetas de direcciones
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;
            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            Vector3 labelPos = pos + dir * radius;

            // Color para principales y diagonales
            Handles.color = (i % 2 == 0) ? mainColor : secondaryColor;

            GUIStyle style = new GUIStyle();
            style.fontSize = Mathf.RoundToInt(labelSize);
            style.normal.textColor = Handles.color;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = (i % 2 == 0) ? FontStyle.Bold : FontStyle.Normal;

            Handles.Label(labelPos + Vector3.up * 0.2f, directions[i], style);
            Handles.DrawLine(pos, labelPos);
        }

        // Mostrar grados (opcional)
        if (showDegrees)
        {
            Handles.color = degreeColor;
            for (int i = 0; i < 360; i += degreeStep)
            {
                Vector3 dir = Quaternion.Euler(0, i, 0) * Vector3.forward;
                Vector3 tickPos = pos + dir * (radius * 1.1f);
                Handles.DrawLine(pos + dir * (radius * 0.95f), tickPos);

                GUIStyle degStyle = new GUIStyle();
                degStyle.fontSize = Mathf.RoundToInt(labelSize * 0.7f);
                degStyle.normal.textColor = degreeColor;
                degStyle.alignment = TextAnchor.MiddleCenter;

                Handles.Label(tickPos + Vector3.up * 0.1f, $"{i}°", degStyle);
            }
        }

        // Muestra orientación del objeto (rotación Y)
        Handles.color = Color.green;
        Vector3 forward = Quaternion.Euler(0, transform.eulerAngles.y, 0) * Vector3.forward;
        Handles.DrawLine(pos, pos + forward * (radius * 1.2f));
        Handles.Label(pos + forward * (radius * 1.4f), $"RotY: {transform.eulerAngles.y:F1}°");
    }
#endif
}