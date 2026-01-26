using System.Collections;
using UnityEngine;

public class Dithering : MonoBehaviour
{
    [Header("❓ ¿Para qué sirve?")]
    //#pragma es una instrucción del compilador (compiler directive) que le dice a C# que cambie cómo procesa el código.
    //No es una función, no es una variable, no es código que se ejecute en tiempo de juego.
    //Es solo una orden especial que se lee antes de compilar.
#pragma warning disable 0414
        [SerializeField, TextArea(2, 6)]
    string info = "Muestra / Oculta las mallas para que la camara pueda ver el interior de las habitaciones.";
#pragma warning restore 0414

    [Header("🧊 Lista de mallas afectadas por Dithering")]
    [Tooltip("Arrastra aquí los modulos (GameObjects) que quieres que se muestren")]
    [SerializeField] GameObject[] meshList;

    [Header("⚙️ Configuración del Dithering")]
    [Tooltip("El tiempo que tarda en mostrar / ocultar las mallas.")]
    [SerializeField] float _timeToVanish = 0.5f;

    [SerializeField] private Material _trimOpaque;
    [SerializeField] private Material _trimTransparent;

    //Variables privadas.
    float _targetAlpha;
    int _triggerCount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;

        //'_triggerCount' nos indica si el jugador sale del trigger o si esta entro por primera vez en la zona,
        //al pasar a otros triggers evita llamar de nuevo ya que el jugador se encontraba en la zona.
        _triggerCount++;

        if (_triggerCount == 1) CallCoroutineForEachMesh("Hide");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag != "Player") return;

        _triggerCount--;

        if (_triggerCount == 0) CallCoroutineForEachMesh("Show");
    }

    private void CallCoroutineForEachMesh(string action)
    {
        //Evita que hayan en marcha corutinas que estan tratando de mostrar las mallas y a la vez ocultandolas creando un BUG visual.
        StopAllCoroutines();

        //Llamar una corutina por cada malla en el array meshList.
        foreach (GameObject mesh in meshList)
        {
            if (mesh != null) StartCoroutine(ModifyMeshAlpha(mesh, action));
        }
    }

    IEnumerator ModifyMeshAlpha(GameObject mesh, string action)
    {
        Renderer meshRenderer = mesh.GetComponent<Renderer>();

        //'alphaColor' toma el color del material para usarlo como parametro en la interpolación
        Color alphaColor = meshRenderer.material.color;

        //'alphaToApply' tomara el valor interpolado que luego se le aplicara a la malla.
        //'alphaToApply' se le asigna un nuevo color para evitar el error de CS0165 Uso de la variable local no asignada 'alphaToApply'
        Color alphaToApply = new Color(1, 1, 1, 1);

        //Cambiamos '_targetAlpha' si el jugador entra o sale del trigger.
        if (action == "Show")
        {
            meshRenderer.enabled = true;
            _targetAlpha = 1f;
        }
        else if (action == "Hide")
        {
            _targetAlpha = 0f;
            meshRenderer.material = _trimTransparent;
            //meshRenderer.sharedMaterial = _trimTransparent;
            //meshRenderer.material = new Material(_trimTransparent);
        }

        float timer = 0f;

        //Leer el bucle durante (_timeToVanish) segundos
        while (timer <= _timeToVanish)
        {
            //Timer se incrementa en el tiempo.
            timer += Time.deltaTime;

            //'t' toma el valor de timer / _timeToVanish para usarse como parametro de la interpolación.
            float t = timer / _timeToVanish;

            //'alphaToApply.a' cambia su valor a '_targetAlpha' de forma progresiva por el parametro 't'.
            alphaToApply.a = Mathf.Lerp(alphaColor.a, _targetAlpha, t);

            //Asegurarse de tomar el valor exacto del alpha al terminar.
            if (timer >= _timeToVanish)
            {
                alphaToApply.a = _targetAlpha;

                //Si se esta ocultando la geometria, se desactiva el mesh renderer para que no se vea la transparencia por los mapas de texturas
                if (action == "Hide")
                {
                    meshRenderer.enabled = false;
                }
                else if (action == "Show")
                {
                    meshRenderer.material = _trimOpaque;
                    //meshRenderer.sharedMaterial = _trimOpaque;
                    //meshRenderer.material = new Material(_trimOpaque);
                }
            }

            //El color del material de la malla se reemplaza por 'alphaToApply' cada frame.
            meshRenderer.material.color = alphaToApply;

            //Esperamos un frame.
            yield return null;
        }
    }
}