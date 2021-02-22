using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum EItemType
{
    Default,
    Consumable,
    Weapon
}

public class InteractableItemBase : MonoBehaviour
{
    public string Name;
    public Sprite Image;
    public string InteractText = "Press F to pickup the item";
    public EItemType ItemType;

    public WeaponConfig weaponConfig;
    public virtual void OnInteractAnimation(Animator animator)
    {
        animator.SetTrigger("drop_tr");
    }

    public virtual void OnInteract()
    {
    }

    public virtual bool CanInteract(Collider other)
    {
        return true;
    }
}

public class InventoryItemBase : InteractableItemBase //MonoBehaviour, IInventoryItem
{
    /*
    public virtual string Name
    {
        get
        {
            return "_base item_";
        }
    }

    public Sprite _Image = null;

    public Sprite Image
    {
        get
        {
            return _Image;
        }
    }
    */

    public InventorySlot Slot
    {
        get; set;
    }

    public virtual void OnPickup()
    {
        print("OnPickup()");
        gameObject.SetActive(false);
        print(gameObject);
        Collider go_collider = GetComponent<Collider>();
        go_collider.isTrigger = false;
    }



    public virtual void OnDrop()
    {
        print("OnDrop()");
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000))
        {
            gameObject.SetActive(true);
            print(hit.point + " " + hit.point.GetType());
            Vector3 offset_up = new Vector3(0.0f, 1.0f, 0.0f);
            gameObject.transform.position = hit.point + offset_up;
            gameObject.transform.eulerAngles = DropRotation;
            Rigidbody item_rb = GetComponent<Rigidbody>();
            //item_rb.freezeRotation = false;
            //item_rb.useGravity = true;
            item_rb.constraints = RigidbodyConstraints.None;
            Collider go_collider = GetComponent<Collider>();
            go_collider.isTrigger = true;
        }
    }

    public virtual void OnUse()
    {
        print("OnUse()");
        //GameObject Hand = (GameObject as MonoBehaviour);
        //goItem.transform.position = Hand.transform.position;
        print("PickPosition = " + PickPosition);
        transform.localPosition = PickPosition;
        transform.localEulerAngles = PickRotation;
        gameObject.SetActive(true);
        print("gameObject = " + gameObject);
        Rigidbody item_rb = GetComponent<Rigidbody>();
        //item_rb.freezeRotation = true;
        //item_rb.useGravity = false;
        item_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
    }


    public Vector3 PickPosition;
    public Vector3 PickRotation;
    public Vector3 DropRotation;

    public bool UseItemAfterPickup = false;


}
