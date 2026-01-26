using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BindButtonToManager : MonoBehaviour
{
    private enum UIActions { Objectives, Notes, Inventory, Exit }
    [SerializeField] private UIActions _action;

    private void Start()
    {
        //Se llama a la función pública 'RegisterButton' de la Instancia estatica de UIManager.
        UIManager.Instance.RegisterButton(_action.ToString(), GetComponent<Button>());
    }
}
