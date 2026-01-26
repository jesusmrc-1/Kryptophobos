using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public Sprite Icon;
    public bool Stackable;
    public int Amount;
}
