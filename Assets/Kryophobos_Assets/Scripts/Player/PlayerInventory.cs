using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private GameObject _flashlightGameObject;
    public Flashlight Flashlight;
    public List<ItemStack> Items = new List<ItemStack>();
    public List<NoteData> Notes = new List<NoteData>();

    public bool HasFlashlight = false;

    private void Start()
    {
        Flashlight = _flashlightGameObject.GetComponent<Flashlight>();

        LoadStatus();
    }

    public void AddItem(ItemData newItem)
    {
        if (newItem.Stackable)
        {
            //Buscamos si existe el Scriptable Object (Item) comparandolo con el que se le ha pasado como parametro (newItem) 
            //Find internamente va comprobando cada elemento como en un bucle foreach en el cual 'i' es cada dato guardado en la lista ItemStack.
            var existing = Items.Find(i => i.Item == newItem);
            if (existing != null)
            {
                existing.Amount += newItem.Amount;
                //Llamamos a la accion que es un DELEGATE, si fuera un event solo se puede llamar a la accion desde este script
                //y si hay alguna funcion suscrita de otro script, tambien se llamara.
                //OnInventoryChanged?.Invoke(Items);
                return;
            }
        }

        //Con el constructor de ItemStack
        /*
        public ItemStack(ItemData item, int amount)
        {
            this.Item = item;
            this.Amount = amount;
        }
         */
        //Evitamos tener que decirle que objeto y cantidad se le añade
        //ItemStack caja = new ItemStack();
        //caja.Item = newItem;
        //caja.Amount = 1;
        //Items.Add(caja);

        //Y nos deja crea la caja ya rellena en una sola línea:
        Items.Add(new ItemStack(newItem, newItem.Amount));

        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerItems.Add(new ItemStack(newItem, newItem.Amount));
        }

        //Para futuro cuando UI Manager este suscrito al evento y asi refrescar la información
        //OnInventoryChanged?.Invoke(Items);

        Debug.Log("Inventario actual:");
        foreach (var stack in Items)
        {
            Debug.Log($"{stack.Item.ItemName} x{stack.Amount}");
        }
    }

    /*“Busca en la lista algún ItemStack cuya propiedad Item sea igual al ScriptableObject item.
    Si encuentra al menos uno, devuelve TRUE.
    Si no hay ninguno, devuelve FALSE.”
     */
    public bool HasItem(ItemData item)
    {
        return Items.Any(i => i.Item == item);
    }

    public void HideFlashlight()
    {
        if (Flashlight != null) _flashlightGameObject.SetActive(false);
    }

    public void ShowFlashlight()
    {
        if (Flashlight != null) _flashlightGameObject.SetActive(true);
    }

    //Llamar desde cada puzle, cada uno sabe si esta hecho, luego le dicen al inventario del jugador, oye, si estoy, entonces borra los items
    private void LoadStatus()
    {
        HasFlashlight = GameManager.Instance.PlayerHasFlashlight;

        Items.Clear();
        Notes.Clear();

        //Si se hace Items = _gameManager.PlayerItems, entonces ERROR => ambas variables apuntan a la MISMA lista en memoria.
        Items = new List<ItemStack>(GameManager.Instance.PlayerItems);
        Notes = new List<NoteData>(GameManager.Instance.PlayerNotes);
    }
}
