using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{

    public Inventory Inventory;
    public GameObject MessagePanel;

    // Start is called before the first frame update
    void Start()
    {
        Inventory.ItemAdded += Inventory_ItemAdded;
        Inventory.ItemRemoved += Inventory_ItemRemoved;
    }

    private void Inventory_ItemAdded(object sender, InventoryEventArgs e)
    {
        Transform inventoryPanel = transform.Find("Inventory");
        //inventoryPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = e.Item.Image;
        int index = -1;
        
        foreach (Transform slot in inventoryPanel)
        {
            index++;
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform textTransform = slot.GetChild(0).GetChild(1);
            Image image = imageTransform.GetComponent<Image>();
            Text txtCount= textTransform.GetComponent<Text>();
            print("txtCount: " + txtCount);
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
            if (index == e.Item.Slot.Id)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;
                int itemCount = e.Item.Slot.Count;
                if (itemCount > 1)
                    txtCount.text = itemCount.ToString();
                else
                    
                    txtCount.text = "";
                itemDragHandler.Item = e.Item;
                break;
            }
        }
       
    }


    private void Inventory_ItemRemoved(object sender, InventoryEventArgs e)
    {
        print("Inventory_ItemRemoved");
        Transform inventoryPanel = transform.Find("Inventory");
        //inventoryPanel.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = e.Item.Image;
        int index = -1;
        foreach (Transform slot in inventoryPanel)
        {
            index++;
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Transform textTransform = slot.GetChild(0).GetChild(1);
            Image image = imageTransform.GetComponent<Image>();
            Text txtCount = textTransform.GetComponent<Text>();
            print("txtCount = " + txtCount);
            ItemDragHandler itemDragHandler = imageTransform.GetComponent<ItemDragHandler>();
            if (itemDragHandler.Item == null) {
                continue;
            }
            if (e.Item.Slot.Id == index)
            {
                int itemCount = e.Item.Slot.Count;
                itemDragHandler.Item = e.Item.Slot.FirstItem;

                if (itemCount < 2)
                    txtCount.text = "";
                else
                    txtCount.text = itemCount.ToString();
                if (itemCount == 0)
                {
                    image.enabled = false;
                    image.sprite = null;
                }
                break;
            }

            //if (itemDragHandler.Item.Equals(e.Item))
            //{
            //    image.enabled = false;
            //    image.sprite = null;
            //    itemDragHandler.Item = null;
            //    break;
            //}
        }

    }

    private bool mIsMessagePanelOpened = false;
    public bool IsMessagePanelOpened
    {
        get { return mIsMessagePanelOpened; }
    }
    public void OpenMessagePanel(InteractableItemBase item)
    {
        MessagePanel.SetActive(true);
        Text mpText = MessagePanel.transform.Find("Text").GetComponent<Text>();
        mpText.text = item.InteractText;
        mIsMessagePanelOpened = true;
    }
    public void OpenMessagePanel(string text)
    {
        MessagePanel.SetActive(true);
        Text mpText = MessagePanel.transform.Find("Text").GetComponent<Text>();
        mpText.text = text;
        mIsMessagePanelOpened = true;
    }

    public void CloseMessagePanel()
    {
        MessagePanel.SetActive(false);
    }

}
