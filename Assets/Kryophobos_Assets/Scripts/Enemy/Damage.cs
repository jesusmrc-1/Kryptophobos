using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private float _damageAmount;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger attack collided");

        //Si other tiene algun script que hereda de la interface IDamageable, entonces se puede llamar a su función.
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.Damage(_damageAmount);
        }
    }
}
