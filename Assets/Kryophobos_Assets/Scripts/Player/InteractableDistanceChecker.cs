using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//IMPORTANTE, EL PARENT SIEMPRE ES EMPTY CON SCRIPT Y RIGIDBODY PARA PODER DETECTAR ON TRIGGER, LUEGO COMO HIJO UN TRIGGER, ENTONCES EL INTERACTABLE 

//Funcionamiento: 

public class InteractableDistanceChecker : MonoBehaviour
{
    [Header("❓ ¿Que hace este script?")]
#pragma warning disable 0414
    [TextArea(1, 3)] [SerializeField] private string _info;
#pragma warning restore 0414

    #region Private Vars
    private Player _player;
    private List <Collider> _nearbyTriggersList = new List<Collider>();
    #endregion

    #region Public Vars
    public Collider ClosestTrigger;

    public Action<Collider> OnPlayerNearTrigger;
    public Action OnNoTriggersNearby;
    #endregion

    private void Start()
    {
        _info = "Este script permite detectar los triggers con Layer = Interactable, el jugador interactuará con el trigger que este mas cerca de él.";

        //Referenciar clases permanentes en escena.
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        //Ocultar el texto cuando no haya ningun trigger cerca.
        HideTextIfNoTriggersNearby();

        //Quitar referencia nula de la lista si el trigger es destruido.
        RemoveNullReferencesFromList();

        //Le decimos el unico objeto detectado
        if (_nearbyTriggersList.Count == 1)
        {
            ClosestTrigger = _nearbyTriggersList[0]; //COMPROBAR PARA QUE ES ESTA LINEA YA QUE PARECE NO SER NECESARIA
            if (ClosestTrigger != null)
            {
                OnPlayerNearTrigger?.Invoke(ClosestTrigger);
                CheckWhatIsClosestToPlayer();
            }
        }

        //Si hay almenos dos triggers cercanos, se hace la comprobación.
        else if (_nearbyTriggersList.Count >= 2)
        {
            //Creamos variable lastDistance que almacenara la ultima distancia del objeto mas cercano al jugador.
            float lastDistance = Mathf.Infinity;

            foreach (Collider c in _nearbyTriggersList)
            {
                //Comprobamos la distancia que hay del jugador al objeto
                float distance = Vector3.Distance(transform.position, c.transform.position);

                //Si la distancia del objeto detectado es menor a la distancia del objeto comprobado anteriormente.
                if (distance < lastDistance)
                {
                    //Guardamos la distancia del ultimo objeto en lastDistance y referenciamos el objeto en ClosestTrigger.
                    lastDistance = distance;
                    ClosestTrigger = c;
                }
            }
            //La función de UI Manager que esta suscrita se ejecutara pasandole un collider como parametro, se usara para posicionar el texto
            //SI ES NECESARIO, VER SI SE PUEDE PASAR OTRO PARAMETRO CAMBIANDO LA ACCION Y ASI PODER CONFIGURAR EL OFFSET EN CADA OBJETO DETECTADO
            if (ClosestTrigger != null)
            {
                OnPlayerNearTrigger?.Invoke(ClosestTrigger);
                CheckWhatIsClosestToPlayer();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;
        _nearbyTriggersList.Add(other);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Interactable")) return;
        _nearbyTriggersList.Remove(other);
    }


    void HideTextIfNoTriggersNearby()
    {
        if (_nearbyTriggersList.Count == 0) 
        {
            _player.IsPlayerNearItem = false;
            _player.IsPlayerNearNote = false;
            _player.IsPlayerNearDoor = false;
            _player.IsPlayerNearPuzzle = false;
            OnNoTriggersNearby?.Invoke();
        }
    }

    void RemoveNullReferencesFromList()
    {
        //lista.RemoveAll(elemento => condicionSobreEseElemento);
        _nearbyTriggersList.RemoveAll(trigger => trigger == null);

        //Si al quitar null references de la lista de triggers se queda vacía, indicamos que no hay ningún trigger cerca.
        if (_nearbyTriggersList.Count == 0) ClosestTrigger = null;
    }

    void CheckWhatIsClosestToPlayer()
    {
        //Si el objeto mas cercano del jugador tiene el tag Item entonces IsPlayerNearItem true, si tiene otro tag como Door, entonces IsPlayerNearItem false
        if (ClosestTrigger.gameObject.tag == "Item") _player.IsPlayerNearItem = true;
        else _player.IsPlayerNearItem = false;

        if (ClosestTrigger.gameObject.tag == "Note") _player.IsPlayerNearNote = true;
        else _player.IsPlayerNearNote = false;

        if (ClosestTrigger.gameObject.tag == "Door") _player.IsPlayerNearDoor = true;
        else _player.IsPlayerNearDoor = false;

        if (ClosestTrigger.gameObject.tag == "Puzzle") _player.IsPlayerNearPuzzle = true;
        else _player.IsPlayerNearPuzzle = false;


    }

    //Orden de llamada PlayerInteractState.cs -> InteractableDistanceChecker.cs -> ItemPickup.cs
    public void PickupItem()
    {
        //IMPORTANTE, el parent es el que tiene la clase ItemPickup, por eso GetComponentInParent y en caso de usar OnTrigger events asegurarse
        //de que el GameObject tiene un Rigidbody para detectar colisiones.

        //Le decimos al objeto que queremos cogerlo.
        ItemPickup itemPickup = ClosestTrigger.GetComponentInParent<ItemPickup>();
        if (itemPickup != null) itemPickup.PickupItem();
    }

    public void PickupNote()
    {
        NotePickup notePickup = ClosestTrigger.GetComponentInParent<NotePickup>();
        if (notePickup != null) notePickup.PickupNote();
    }
}
