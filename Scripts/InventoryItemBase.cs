using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemBase : MonoBehaviour, IInventoryItem
{

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

    public virtual void OnPickup()
    {
        print("picked");
        gameObject.SetActive(false);
        print(gameObject);
    }


    public virtual void OnDrop()
    {
        print("dropped123");
        RaycastHit hit = new RaycastHit();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 1000))
        {
            gameObject.SetActive(true);
            print(hit.point + " " + hit.point.GetType());
            Vector3 offset_up = new Vector3(0.0f, 1.0f, 0.0f);
            gameObject.transform.position = hit.point + offset_up;
        }
    }

    public virtual void OnUse()
    {

    }

        // Start is called before the first frame update
        void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
