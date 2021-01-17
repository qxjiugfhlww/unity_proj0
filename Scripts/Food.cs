using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : InventoryItemBase
{
    public int FoodPoints = 20;

    public override void OnUse()
    {
        GameManager.Instance.Player.health.Eat(FoodPoints);
        GameManager.Instance.Player.itemInterract.inventory.RemoveItem(this);
        Destroy(this.gameObject);
    }
}
