using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RoomWaypoints : MonoBehaviour
{
    [SerializeField] private List<GameObject> _roomWaypoints = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null)
            {
                //enemy.RoomWaypoints.Clear();
                //enemy.RoomWaypoints = _roomWaypoints;

                if (!enemy.StartSearching) enemy.RoomWaypoints = new List<GameObject>(_roomWaypoints);
            }
        }
    }
}
