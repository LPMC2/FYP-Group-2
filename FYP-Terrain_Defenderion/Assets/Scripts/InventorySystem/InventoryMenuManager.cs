using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuManager : MonoBehaviour
{
    [SerializeField] private Sprite normalSlotTypeImg;
    [SerializeField] private Sprite selectedSlotTypeImg;
    [SerializeField] private float selectedSizeY = 10f;
    [SerializeField] private List<InvMenu> invMenu = new List<InvMenu>();
    public List<InvMenu> InventoryMenu { get { return invMenu; } }
    public void AddInvMenu(int id, Image target)
    {
        invMenu.Add(new InvMenu(id, target));
    }
    public void SelectMenuID(int id)
    {
        foreach(InvMenu inventoryMenu in invMenu)
        {
            inventoryMenu.TargetSlotType.sprite = normalSlotTypeImg;
        }
        invMenu[id].TargetSlotType.sprite = selectedSlotTypeImg;
    }
    [System.Serializable]
    public class InvMenu
    {
        private int id = -1;
        private Image targetSlotType;
        public int ID { get { return id; }set { id = value; } }
        public InvMenu (int id, Image image)
        {
            this.id = id;
            targetSlotType = image;
        }
        public Image TargetSlotType { get { return targetSlotType; } set { targetSlotType = value; } }
    }
}
