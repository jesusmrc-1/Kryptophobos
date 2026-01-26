using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] float _blindTime;
    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.CompareTag("Enemy")) COMPROBAR CAPA ENEMIGO PORQUE SI NO TRATA DE COGER COGER LA CLASE ENEMY DE CUALQUIER COLLIDER
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
        Debug.LogWarning("Enemigo flasheado");
        Enemy enemy = other.GetComponent<Enemy>();

        enemy.StartCoroutine(enemy.Blind(_blindTime));
    }
}
