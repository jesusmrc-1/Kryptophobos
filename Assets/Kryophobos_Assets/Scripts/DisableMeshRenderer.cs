using UnityEngine;

public class DisableMeshRenderer : MonoBehaviour
{
    [Header("❓ ¿Para qué sirve?")]
    //#pragma es una instrucción del compilador (compiler directive) que le dice a C# que cambie cómo procesa el código.
    //No es una función, no es una variable, no es código que se ejecute en tiempo de juego.
    //Es solo una orden especial que se lee antes de compilar.
#pragma warning disable 0414
    [SerializeField, TextArea(2, 6)]
    private string _info = "Un simple script que desactiva el componente 'Mesh Renderer', de esta forma no veremos los triggers in-game.";
#pragma warning restore 0414

    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshRenderer.enabled = false;
    }
}
