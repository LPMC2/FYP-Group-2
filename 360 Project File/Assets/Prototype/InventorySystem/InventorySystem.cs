using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour
{

    [Header("Inventory Settings")]
    [SerializeField] 
    private Backpack backpack;
    [SerializeField] 
    private int maxFrontSlots = 9;
    public int[] initialSlotItem;

    [Header("Inventory UI")]
    public GameObject slotPrefab;
    [SerializeField] 
    GameObject SlotPlaceHolder;
    
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
    [SerializeField]
    private ItemSO itemData;
    public BackpackItemData[] backpackItemDatas { get; private set; }
    private void Start()
    {
        backpackItemDatas = new BackpackItemData[slot.Length];
        backpack.ContentTransform.transform.GetChild(0).GetComponent<VerticalLayoutGroup>().spacing = -backpack.ContentTransform.GetChild(0).GetComponent<RectTransform>().rect.height / 2f;
        setSlotUI();
        setInitialInventory();
    }

    #region Function
    private void setSlotUI()
    {
        for (int i = 0; i < slot.Length; i++)
        {
            AddSlot();
        }
    }
    public void AddSlot()
    {
        GameObject ui = Instantiate(slotPrefab, SlotPlaceHolder.transform.position, Quaternion.identity);
        ui.transform.SetParent(SlotPlaceHolder.transform);
        RectTransform rectTransform = ui.GetComponent<RectTransform>();
        Vector2 clampedSizeDelta = rectTransform.sizeDelta;
        clampedSizeDelta.x = Mathf.Clamp(clampedSizeDelta.x, 0, backpack.ContentTransform.rect.width);
        rectTransform.sizeDelta = clampedSizeDelta;
        rectTransform.localScale = Vector3.one;

    }
    private void setInitialInventory()
    {
        for (int i = 0; i < initialSlotItem.Length; i++)
        {
            setSlotItem(i, initialSlotItem[i]);
        }
    }

    public void setSlotItem(int id, int item)
    {
        if (item >= itemData.item.Length || id >= slot.Length)
        {
            return;
        }
        //Debug.Log(id + ":" + item + "\n" + slot[slot.Length - 1] + "\n" + slot[slot.Length - 2].getId());
        slot[id].setId(item);
        if (item != -1 && item < itemData.item.Length)
        {
            slot[id].item.SlotUI.itemSprite = itemData.item[item].itemSprite;
            slot[id].setName(itemData.item[item].GetItemName());
            BackpackItemData backpackItemData = SlotPlaceHolder.transform.GetChild(id).GetComponent<BackpackItemData>();
            if(backpackItemData != null)
            {
                backpackItemData.SetItemId(item);
                backpackItemData.SetItemObject(backpackItemData.gameObject);
            }
            backpackItemDatas[id] = backpackItemData;
        }else
        if(item == -1)
        {
            slot[id].item.SlotUI.itemSprite = null;
            slot[id].setName(null);
            BackpackItemData backpackItemData = SlotPlaceHolder.transform.GetChild(id).GetComponent<BackpackItemData>();
            if (backpackItemData != null)
            {
                backpackItemData.SetItemId(item);
            }
        }
        
        setSlotUI(id);

    }
    private void setSlotUI(int id)
    {

        if (id < SlotPlaceHolder.transform.childCount)
        {
            Image image = GameObjectFinder.GetGameObjectWithTagFromChilds(SlotPlaceHolder.transform.GetChild(id).gameObject, "Image").GetComponent<Image>();
            TMP_Text itemText = GameObjectFinder.GetGameObjectWithTagFromChilds(SlotPlaceHolder.transform.GetChild(id).gameObject, "Text").GetComponent<TMP_Text>();
            image.sprite = slot[id].item.SlotUI.itemSprite;
            
            itemText.text = slot[id].getName();

        }

    }
    public void ChangeName(int languageIndex)
    {
       
        for (int id = 0; id < slot.Length; id++)
        {
            Debug.Log(itemData.item[slot[id].getId()].GetItemName(languageIndex+1));
            slot[id].setName(itemData.item[slot[id].getId()].GetItemName(languageIndex+1));
            TMP_Text itemText = GameObjectFinder.GetGameObjectWithTagFromChilds(SlotPlaceHolder.transform.GetChild(id).gameObject, "Text").GetComponent<TMP_Text>();
            itemText.text = slot[id].getName();
        }

    }
    #endregion

    #region Slot Class
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
        [SerializeField] private SlotUI slotUI;
        public SlotUI SlotUI { get { return slotUI;} set { slotUI = value; } }
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
        public Sprite itemSprite { get { return itemImage; } set { itemImage = value; } }
    }
    #endregion
}
