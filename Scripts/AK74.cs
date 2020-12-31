using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AK74 : MonoBehaviour, IInventoryItem
{
    public string Name
    {
        get
        {
            return "AK74";
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

    public void OnPickup()
    {
        gameObject.SetActive(false);
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
