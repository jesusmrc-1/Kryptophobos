using UnityEngine;
using System.Collections;

public class MovimientoAleatorio : MonoBehaviour
{
    [Header("Configuración")]
    public float radio = 5f;         // Tamaño de la esfera
    public float velocidad = 3f;     // Velocidad de movimiento
    public float pausa = 1f;         // Tiempo de espera al llegar

    private Vector3 centroInicial;   // Centro fijo de la esfera
    private Vector3 puntoDestino;

    void Start()
    {
        // Guardamos la posición inicial como centro de la esfera
        centroInicial = transform.position;

        GenerarNuevoDestino();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, puntoDestino, velocidad * Time.deltaTime);

        if (Vector3.Distance(transform.position, puntoDestino) < 0.1f)
        {
            StartCoroutine(EsperarYNuevoDestino());
        }
    }

    void GenerarNuevoDestino()
    {
        // Punto aleatorio dentro de la esfera que tiene como centro la posición inicial
        Vector3 randomPos = Random.insideUnitSphere * radio;
        puntoDestino = centroInicial + randomPos;
    }

    IEnumerator EsperarYNuevoDestino()
    {
        yield return new WaitForSeconds(pausa);
        GenerarNuevoDestino();
    }
}
