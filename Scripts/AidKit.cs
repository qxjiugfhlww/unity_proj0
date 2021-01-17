using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AidKit : InventoryItemBase
{
    public int HealthPoints = 20;

    public override void OnUse()
    {
        GameManager.Instance.Player.health.Rehab(HealthPoints);
        GameManager.Instance.Player.itemInterract.inventory.RemoveItem(this);
        Destroy(this.gameObject);
    }
  
}
