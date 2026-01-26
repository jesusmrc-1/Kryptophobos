using UnityEngine;

public class TypeOfSurface : MonoBehaviour
{
    [SerializeField] private FootstepSet _footstepSet_A;
    [SerializeField] private FootstepSet _footstepSet_B;

    [Tooltip("Aquí va la referencia del punto 'A'.")]
    [SerializeField] private GameObject a;
    [Tooltip("Aquí va la referencia del punto 'B'.")]
    [SerializeField] private GameObject b;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLayer"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            PlayerFootsteps playerFootsteps = other.GetComponent<PlayerFootsteps>();

            if (player != null && playerFootsteps != null)
            {
                //Comprobar si el jugador esta mas cerca de 'A' o de 'B'

                Vector3 playerPosition = other.transform.position;
                Vector3 a = this.a.transform.position;
                Vector3 b = this.b.transform.position;

                float distanceFromA = Vector3.Distance(playerPosition, a);
                float distanceFromB = Vector3.Distance(playerPosition, b);

                if (distanceFromA < distanceFromB)
                {
                    playerFootsteps.SetCurrentSurface(_footstepSet_A);
                }

                else if (distanceFromB < distanceFromA)
                {
                    playerFootsteps.SetCurrentSurface(_footstepSet_B);
                }
            }
        }
    }
}
