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
    [Header("Main Settings")]
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
    }
    public void setup()
    {
        setSlotUI();
        setInitialInventory();
    }
    private void setSlotUI()
    {
        for (int i = 0; i < inventory.slot.Length; i++)
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
        inventory.slot[id].setId(item);
        if (item != -1 && item < itemData.item.Length)
        {
            inventory.slot[id].item.SetItem(itemData.item[item].itemObject);
        }
        setSlotUI(id);
        if (id == inventory.getSelectedSlot())
        {
            EquipItem(id);
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
        if (inventory.getSelectedSlot() != slotId)
        {
            inventory.setSelectedSlot(slotId);
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
            itemName = itemData.item[inventory.slot[itemId].getId()].itemName;
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
