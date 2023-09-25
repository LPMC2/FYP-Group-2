using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
  
    [Header("Inventory")]
    [SerializeField] private int selectedSlot = 1;
    [SerializeField] private int maxFrontSlots = 9;
    public Slot[] slot;
    public int getSelectedSlot()
    {
        return selectedSlot;
    }
    public void setSelectedSlot(int id)
    {
        selectedSlot = id;
    }

    [System.Serializable]
    public class Slot
    {
        public Item item;
        [SerializeField] int count;
        [SerializeField] string name;
        [SerializeField] int itemId;
        public int getCount()
        {
            return count;
        }
        public int getId()
        {
            return itemId;
        }
        public void setId(int id)
        {
            itemId = id;
        }
        public void setCount(int value)
        {
            count = value;
        }
        public void setName(string value)
        {
            name = value;
        }
        public string getName()
        {
            return name;
        }
    }
    [System.Serializable]
    public class Item
    {
        [SerializeField] private GameObject itemObject;
        public GameObject GetItem()
        {
            return itemObject;
        }
        public void SetItem(GameObject item)
        {
            itemObject = item;
        }
    }
    [System.Serializable]
    public class SlotUI
    {
        [SerializeField] private Sprite itemImage;
        public void setSlotUI(Sprite image)
        {
            itemImage = image;
        }
    }
}
