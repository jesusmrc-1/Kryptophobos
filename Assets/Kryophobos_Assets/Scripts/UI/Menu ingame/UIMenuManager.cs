using System.Collections.Generic;
using UnityEngine;

public class UIMenuManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _uiGameObjects = new List<GameObject>();

    //Guardamos las referencias de las ventanas de la UI del menu ingame en '_uiGameObjects'
    public void SaveRefsFromMenuIngame(GameObject uiGameObject)
    {
        //foreach ()
        _uiGameObjects.Add(uiGameObject);
    }
}
