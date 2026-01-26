using UnityEngine;

public class ItemStack
{
    public ItemData Item;
    public int Amount;

    //CONSTRUCTO
    public ItemStack(ItemData item, int amount)
    {
        this.Item = item;
        this.Amount = amount;
    }
}
