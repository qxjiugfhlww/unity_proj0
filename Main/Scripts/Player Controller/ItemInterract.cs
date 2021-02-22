using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInterract : MonoBehaviour
{

    public GameObject Hand;
    public HUD Hud;
    public Inventory inventory;
    internal InventoryItemBase mCurrentItem = null;
    internal bool mLockPickup = false;
    internal InteractableItemBase mInteractItem = null;
    public Animator animator;


    public void Init()
    {
        inventory.ItemUsed += Inventory_ItemUsed;
        inventory.ItemRemoved += Inventory_ItemRemoved;
    }

    public bool CarriesItem(string itemName)
    {
        if (mCurrentItem == null)
            return false;

        return (mCurrentItem.Name == itemName);
    }



    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        //IInventoryItem
        InventoryItemBase item = e.Item;

        GameObject goItem = (item as MonoBehaviour).gameObject;
        goItem.SetActive(true);
        goItem.transform.parent = null;
    }

    private void SetItemActive(InventoryItemBase item, bool active)
    {
        GameObject currentItem = (item as MonoBehaviour).gameObject;
        currentItem.SetActive(active);
        currentItem.transform.parent = active ? Hand.transform : null;
    }


    private void Inventory_ItemUsed(object sender, InventoryEventArgs e)
    {

        //if (e.Item.ItemType != EItemType.Consumable)
        //{
        print("Inventory_ItemUsed");
        print("mCurrentItem = " + mCurrentItem);
        if (mCurrentItem != null)
        {
            SetItemActive(mCurrentItem, false);
        }
        InventoryItemBase item = e.Item;
        SetItemActive(item, true);
        mCurrentItem = e.Item;
        //mCurrentItem.transform.parent = Hand.transform;
        //}

        /*
			//if (mCurrentItem != null)
			//{
			//	SetItemActive(mCurrentItem, false);
			//}
			//InventoryItemBase item = e.Item;

			//SetItemActive(item, true);

			///
			//GameObject goItem = (item as MonoBehaviour).gameObject;
			//goItem.SetActive(true);
			//goItem.transform.parent = Hand.transform;
			///

			//mCurrentItem = e.Item;
			///
			////goItem.transform.position = Hand.transform.position;
			//goItem.transform.localPosition = (item as InventoryItemBase).PickPosition;
			//goItem.transform.localEulerAngles = (item as InventoryItemBase).PickRotation;
			//Rigidbody item_rb = (item as InventoryItemBase).GetComponent<Rigidbody>();
			//item_rb.freezeRotation = true;
			//item_rb.useGravity = false;
			///

		*/

    }



    private void DropCurrentItem()
    {
        mLockPickup = true;
        animator.SetTrigger("drop_tr");
        GameObject goItem = (mCurrentItem as MonoBehaviour).gameObject;
        inventory.RemoveItem(mCurrentItem);
        mCurrentItem.OnDrop();

        //Rigidbody rbItem = goItem.AddComponent<Rigidbody>();
        Rigidbody rbItem = goItem.GetComponent<Rigidbody>();
        if (rbItem != null)
        {
            print("gdfgdfgdfg " + transform.forward);
            rbItem.AddForce(transform.forward * 1.0f, ForceMode.Impulse);
            Invoke("DoDropItem", 0.25f);
        }
    }
    public void DoDropItem()
    {
        mLockPickup = false;
        if (mCurrentItem != null)
        {

            print("can pickup " + mCurrentItem.Name);
            //Destroy((mCurrentItem as MonoBehaviour).GetComponent<Rigidbody>());
            mCurrentItem = null;
        }
    }


    private bool mIsControlEnabled = true;
    public void EnableControl()
    {
        mIsControlEnabled = true;
    }
    public void DisableControl()
    {
        mIsControlEnabled = false;
    }


    public void InteractWithItem()
    {
        print("public void InteractWithItem()");
        if (mInteractItem != null)
        {
            mInteractItem.OnInteract();

            if (mInteractItem is InventoryItemBase)
            {
                InventoryItemBase inventoryItem = mInteractItem as InventoryItemBase;

                inventory.AddItem(inventoryItem);
                inventoryItem.OnPickup();

                if (inventoryItem.UseItemAfterPickup)
                {
                    inventory.UseItem(inventoryItem);
                    //inventoryItem.OnUse();


                }
                Hud.CloseMessagePanel();
                mInteractItem = null;
            }
            //else
            //{
            //    if (mInteractItem.ContinueInteract())
            //    {
            //        Hud.OpenMessagePanel(mInteractItem);
            //    }
            //    else
            //    {
            //        Hud.CloseMessagePanel();
            //        mInteractItem = null;
            //    }
            //}
        }
    }

    private void OnTriggerEnter(Collider other) //OnCollisionEnter
    {
        print("OnTriggerEnter TryInteraction");
        TryInteraction(other);
    }

    private void TryInteraction(Collider other)
    {
        InteractableItemBase item = other.GetComponent<InteractableItemBase>();

        if (item != null)
        {
            if (item.CanInteract(other))
            {
                mInteractItem = item;

                Hud.OpenMessagePanel(mInteractItem);
            }
        }

        /*
		if (item != null)
		{
			if (mLockPickup)
				return;
			mItemToPickup = item;
			//inventory.AddItem(item);
			//item.OnPickup();
			Hud.OpenMessagePanel("");
		}
		*/
    }

    private void OnTriggerExit(Collider other)
    {

        InventoryItemBase item = other.GetComponent<InventoryItemBase>();
        if (item != null)
        {
            Hud.CloseMessagePanel();
            mInteractItem = null;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        /*
		if (mLockPickup)
			return;
		IInventoryItem item = hit.collider.GetComponent<IInventoryItem>();
		if (item != null)
		{
			inventory.AddItem(item);
		}
		*/
    }

}
