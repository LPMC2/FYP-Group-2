using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryLoadoutUIManager : MonoBehaviour
{
    [SerializeField] private List<Loadout> m_inventoryLoadouts = new List<Loadout>();
    [SerializeField] private GameObject m_slotUIPrefab;
    [SerializeField] private Transform m_slotPlaceLocation;
    [SerializeField] private TMP_Text itemSlotName;
    [SerializeField] private List<ObjectRotateAroundBehaviour> m_rotators = new List<ObjectRotateAroundBehaviour>();
    [SerializeField] private int m_currentLoadout = 0;
    [SerializeField] private InventoryLoadoutImageSaver inventoryLoadoutImageSaver;
    [SerializeField] private InventoryBehaviour playerInventory;
    // Start is called before the first frame update
    void Start()
    {
        if(inventoryLoadoutImageSaver ==null)
        {
            inventoryLoadoutImageSaver = gameObject.GetComponent<InventoryLoadoutImageSaver>();
        }
        inventoryLoadoutImageSaver.StartCapture();
        LoadLoadout();
    }
    public void AddNumPage(int num)
    {
        m_currentLoadout += num;
        if(m_currentLoadout >= m_inventoryLoadouts.Count)
        {
            m_currentLoadout = 0;
        }
        if(m_currentLoadout < 0)
        {
            m_currentLoadout = m_inventoryLoadouts.Count - 1;
        }
        LoadLoadout();
    }
    public void SetInvData()
    {
        playerInventory.InitialSlotItem = arrayBehaviour.ListToArray(m_inventoryLoadouts[m_currentLoadout].loadoutSO.slot);
        //Require Fix
        InventorySystem.Slot.Resize(ref playerInventory.inventory.slot, m_inventoryLoadouts[m_currentLoadout].loadoutSO.slot.Count);
        playerInventory.inventory.SetMaxFrontSlots(m_inventoryLoadouts[m_currentLoadout].loadoutSO.slot.Count);
        
    }
    public void LoadLoadout()
    {
        foreach(ObjectRotateAroundBehaviour objectRotateAround in m_rotators)
        {
            objectRotateAround.StartRotateAngle(objectRotateAround.transform.GetChild(0));
        }
        GameObjectExtension.RemoveAllObjectsFromParent(m_slotPlaceLocation);
        InventoryLoadoutSO loadout = m_inventoryLoadouts[m_currentLoadout].loadoutSO;
        itemSlotName.text = loadout.name;
        foreach(int id in loadout.slot)
        {
            GameObject itemSlot = Instantiate(m_slotUIPrefab, m_slotPlaceLocation.transform.position, Quaternion.identity, m_slotPlaceLocation.transform);
            Image img = GameObjectExtension.GetGameObjectWithTagFromChilds(itemSlot, "SlotImg").GetComponent<Image>();
            img.sprite = inventoryLoadoutImageSaver.ItemSprites[id];
            SimpleTooltip simpleTooltip = itemSlot.GetComponent<SimpleTooltip>();
            simpleTooltip.infoLeft = loadout.ItemScriptableObject.item[id].GetInfo();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    [System.Serializable]
    public struct Loadout
    {
        public InventoryLoadoutSO loadoutSO;
        public GameObject DisplayObject;
    }
}
