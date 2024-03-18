using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    [System.Serializable]
    public class Slot
    {
        public Item item;
        [SerializeField] int count;
        [SerializeField] string name;
        [SerializeField] int itemId;
        [SerializeField] private TMP_Text itemDisplay;
        public TMP_Text ItemDisplay { get { return itemDisplay; } set { itemDisplay = value; } }
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
        public static void Resize(ref Slot[] targetArray, int newSize)
        {
            targetArray = new Slot[newSize];
            for(int i=0; i<targetArray.Length; i++)
            {
                targetArray[i] = new Slot();
                targetArray[i].item = new Item();
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
