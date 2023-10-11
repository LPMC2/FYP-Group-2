using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
  
    [Header("Inventory")]
    [SerializeField] private int maxFrontSlots = 9;
    #region maxFrontSlots getter
    public int GetMaxFrontSlots()
    {
        return maxFrontSlots;
    }
    public void SetMaxFrontSlots(int value)
    {
        maxFrontSlots = value;
    }
    #endregion
    public Slot[] slot;
    //public int getSelectedSlot()
    //{
    //    return selectedSlot;
    //}
    //public void setSelectedSlot(int id)
    //{
    //    selectedSlot = id;
    //}
    public Slot[] GetSlots()
    {
        return slot;
    }
    [System.Serializable]
    public class Slot
    {
        public Item item;
        [SerializeField] int count = 0;
        [SerializeField] string name = "";
        [SerializeField] int itemId = -1;
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
            if (name != null || name != "")
            {
                return name;
            } else
            {
                return item.GetItem().name;
            }
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
