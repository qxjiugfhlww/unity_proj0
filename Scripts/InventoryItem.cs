using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public interface IInventoryItem
{
    string Name { get; }
    Sprite Image { get; }
    void OnPickup();
    void OnDrop();
    void OnUse();

    InventorySlot Slot { get; set; }
}
*/

public class InventoryEventArgs : EventArgs
{
    /*
    public InventoryEventArgs(IInventoryItem item)
    {
        Item = item;
    }
    public IInventoryItem Item;
    */
    public InventoryEventArgs(InventoryItemBase item)
    {
        Item = item;
    }
    public InventoryItemBase Item;
}

/*
public class InventoryItem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
*/
