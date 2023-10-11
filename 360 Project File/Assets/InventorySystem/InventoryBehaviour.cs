using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryBehaviour : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject SlotPlaceHolder;
    [Header("Slot UI")]
    public Sprite slotImageNormal;
    public Sprite slotImageSelected;
    [Header("Item Text Display")]
    public TMP_Text DisplayText;
    public float fadeInDuration = 1f; // Duration of the fade in animation
    public float displayDuration = 2f; // Duration to display the text
    public float fadeOutDuration = 1f; // Duration of the fade out animation
    private Coroutine fadeCoroutine;
    [Header("Mouse Input")]
    [SerializeField] private float scrollCoolDown = 0.2f;
    private float scrollTimer = 0f; // Timer to track cooldown
    [SerializeField] private bool isScrollable = true;
    [Header("Main Settings")]
    [SerializeField] private int selectedSlot = 1;
    public InventorySystem inventory;
    [SerializeField] private GameObject slotObject;
    [SerializeField] private GameObject placeItemLocation;
    [SerializeField] private int[] initialSlotItem;
    private ItemSO itemData;
    private void Awake()
    {
        itemData = ItemManager.ItemData;
    }

    // Start is called before the first frame update
    void Start()
    {
        #region ShowItem(Debug)
        Debug.Log("Item List: ");
        for (int i = 0; i < itemData.item.Length; i++)
        {
            Debug.Log(itemData.item[i].itemObject);
        }
        #endregion
        if (inventory == null)
        {
            inventory = gameObject.GetComponent<InventorySystem>();
        }
        setup();
    }

    // Update is called once per frame
    void Update()
    {
        inputNumDetection();
    }
    public void inputNumDetection()
    {
        if (inventory.slot.Length > 1)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(i.ToString()))
                {
                    int inputIndex = i - 1; // Adjust the index to match array or list indices

                    // Check if the inventory slot at the input index exists
                    if (inputIndex >= 0 && inputIndex < inventory.slot.Length && inventory.slot[inputIndex] != null)
                    {
                        EquipItem(inputIndex);
                    }
                }
            }
            scrollWheelDetection();
        }
    }
    private void scrollWheelDetection()
    {
        if (scrollTimer <= 0f)
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            if (scrollInput < 0f)
            {
                // Scroll Upward
                // Perform actions or increase a value
                if (selectedSlot < inventory.GetMaxFrontSlots() - 1)
                {
                    selectedSlot++;
                }
                else
                {
                    selectedSlot = 0;
                }
                EquipItem(selectedSlot);
                scrollTimer = scrollCoolDown;
            }
            else if (scrollInput > 0f)
            {
                // Scroll Downward
                // Perform actions or decrease a value
                if (selectedSlot > 0)
                {
                    selectedSlot--;
                }
                else
                {
                    selectedSlot = inventory.GetMaxFrontSlots() - 1;
                }
                EquipItem(selectedSlot);
                scrollTimer = scrollCoolDown;
            }
        }
        else
        {
            // Update the cooldown timer
            scrollTimer -= Time.deltaTime;
        }
    }
    #region Add / Remove Slot Controls
    public void AddSlot()
    {
        GameObject ui = Instantiate(slotObject, SlotPlaceHolder.transform.position, Quaternion.identity);
        ui.transform.SetParent(SlotPlaceHolder.transform);
        if (ui.GetComponent<Image>() == null)
        {
            ui.AddComponent<Image>();
        }
        Image uiImg = ui.GetComponent<Image>();
        uiImg.sprite = slotImageNormal;
        uiImg.preserveAspect = true;

    }
    public void AddNewItemAndSlot(int id)
    {
        //id -> Item Element from ItemSO
        AddSlot();
        if (inventory != null) 
        {
            //Add new slot and assign data into it
            inventory.slot = arrayBehaviour.addArray<InventorySystem.Slot>(inventory.slot);
            
            int slotLength = inventory.slot.Length - 1;
            inventory.slot[slotLength] = new InventorySystem.Slot();
            inventory.slot[slotLength].item = new InventorySystem.Item();
            setSlotItem(slotLength, id);

        }
    }
    public void RemoveSlot(int id)
    {
        if(SlotPlaceHolder.transform.childCount-1 >= id)
        {
            if (inventory != null)
            {
                inventory.slot[id] = null;
                inventory.slot = arrayBehaviour.RemoveArrayElement<InventorySystem.Slot>(inventory.slot, id);
            }
            Destroy(SlotPlaceHolder.transform.GetChild(id).gameObject);


        }
    }

    #endregion
    public void setup()
    {
        setSlotUI();
        setInitialInventory();
    }
    private void setSlotUI()
    {
        for (int i = 0; i < inventory.slot.Length; i++)
        {
            AddSlot();
        }
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
        if(item >= itemData.item.Length || id >= inventory.slot.Length)
        {
            return;
        }
        Debug.Log(id + ":" + item + "\n" + inventory.slot[inventory.slot.Length-1] + "\n" + inventory.slot[inventory.slot.Length - 2].getId());
        inventory.slot[id].setId(item);
        if (item != -1 && item < itemData.item.Length)
        {
            inventory.slot[id].item.SetItem(itemData.item[item].itemObject);
        }
        setSlotUI(id);
        if (id == selectedSlot)
        {
            EquipItem(id);
        }
    }
    public void setEquipItem(int item)
    {
        setSlotItem(selectedSlot, item);
    }
    public void setEquipItem(string item)
    {
        if (!string.IsNullOrEmpty(item) && int.TryParse(item, out _))
        {
            int index = int.Parse(item);
            setSlotItem(selectedSlot, index);
        }
    }
    private void resetSelectUI()
    {
        foreach (Transform child in SlotPlaceHolder.transform)
        {
            Image childImg = child.GetComponent<Image>();
            childImg.sprite = slotImageNormal;
        }
    }

    public void EquipItem(int slotId)
    {
        ItemManager.removeChilds(placeItemLocation);
        resetSelectUI();
        if (selectedSlot != slotId)
        {
            selectedSlot = slotId;
        }
        if (inventory.slot[slotId].getId() >= 0 && inventory.slot[slotId].getId() < inventory.slot.Length)
        {
            Debug.Log(inventory.slot[slotId].getId());
            GameObject targetItem = Instantiate(itemData.item[inventory.slot[slotId].getId()].itemObject, placeItemLocation.transform.position, Quaternion.identity);
            targetItem.transform.SetParent(placeItemLocation.transform);
        }
        Image targetImg = SlotPlaceHolder.transform.GetChild(slotId).GetComponent<Image>();
        targetImg.sprite = slotImageSelected;
        StartFadeInText(slotId);
    }
    private void setSlotUI(int id)
    {

        if (id < SlotPlaceHolder.transform.childCount)
        {

            GameObject targetUI = SlotPlaceHolder.transform.GetChild(id).gameObject;
            if (targetUI.transform.childCount > 0)
            {
                foreach(Transform child in targetUI.transform)
                {
                    Destroy(child.gameObject);
                }
                
            }
            if (inventory.slot[id].getId() != -1 && inventory.slot[id].getId() < itemData.item.Length)
            {
                GameObject instantiatedUI = Instantiate(slotObject, Vector3.zero, Quaternion.identity);
                instantiatedUI.transform.SetParent(targetUI.transform);
                if (instantiatedUI.GetComponent<Image>() == null)
                {
                    instantiatedUI.AddComponent<Image>();
                }
                instantiatedUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                Image uiImg = instantiatedUI.GetComponent<Image>();

                uiImg.sprite = itemData.item[inventory.slot[id].getId()].itemSprite;
                uiImg.preserveAspect = true;
            }
        }

    }
    #region Text Display
    public void StartFadeInText(int itemId)
    {
        // Stop any ongoing fade coroutine
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Get the item name from the item data using the provided item ID
        string itemName = "";
        if (inventory.slot[itemId].getId() != -1)
        {
            itemName = itemData.item[inventory.slot[itemId].getId()].itemName.text;
        }

        // Start the fade coroutine
        fadeCoroutine = StartCoroutine(FadeText(itemName));
    }

    private IEnumerator FadeText(string text)
    {
        // Set the initial text and alpha value
        DisplayText.text = text;
        DisplayText.alpha = 0f;

        // Fade in animation
        float fadeInTimer = 0f;
        while (fadeInTimer < fadeInDuration)
        {
            fadeInTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, fadeInTimer / fadeInDuration);
            DisplayText.alpha = alpha;
            yield return null;
        }

        // Display the text for a duration
        yield return new WaitForSeconds(displayDuration);

        // Fade out animation
        float fadeOutTimer = 0f;
        while (fadeOutTimer < fadeOutDuration)
        {
            fadeOutTimer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, fadeOutTimer / fadeOutDuration);
            DisplayText.alpha = alpha;
            yield return null;
        }

        // Reset the text and alpha value
        DisplayText.text = "";
        DisplayText.alpha = 0f;

        // Reset the fade coroutine reference
        fadeCoroutine = null;
    }

    #endregion
}
