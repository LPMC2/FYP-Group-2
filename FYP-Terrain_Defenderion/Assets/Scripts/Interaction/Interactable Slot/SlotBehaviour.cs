using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SlotBehaviour : MonoBehaviour
{
    InventoryBehaviour inventoryBehaviour;
    private int itemID=-1;
    Button button;
    public void Initialize(InventoryBehaviour inventoryBehaviour, int itemID = default, SlotType slotType = default)
    {
        button = gameObject.GetComponent<Button>();
        this.inventoryBehaviour = inventoryBehaviour;
        this.itemID = itemID;
        
        switch (slotType) 
        {
            case SlotType.InventoryBag:
                button.onClick.AddListener(SetSlotItem);
                break;
            case SlotType.InventorySlot:
                button.onClick.AddListener(Equip);
                break;
            case SlotType.StructureEditor:
                button.onClick.AddListener(SetSlotItem);
                break;
        
        }
    }
    public void SetSlotItem()
    {
        inventoryBehaviour.setSlotItem(inventoryBehaviour.SelectedSlot, itemID);
    }
    public void Equip()
    {
        inventoryBehaviour.EquipItem(itemID);
    }
   
}
public enum SlotType 
{
    Default,
    InventoryBag,
    InventorySlot,
    StructureEditor
}

