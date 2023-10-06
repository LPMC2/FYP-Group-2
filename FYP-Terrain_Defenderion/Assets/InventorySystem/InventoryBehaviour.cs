using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
public enum InventoryType
{
    item,
    block
}
public class InventoryBehaviour : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField] private bool isDropable = false;
    [SerializeField] private bool haveInventoryBag = false;
    [Header("Input Settings")]
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode invBagKey = KeyCode.E;
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
    [Header("Main Settings")]
    [SerializeField] private InventoryType inventoryType; 
    public InventorySystem inventory;
    [SerializeField] private int selectedSlot = 1;
    [SerializeField] private GameObject slotObject;
    [SerializeField] private GameObject placeItemLocation;
    [SerializeField] private int[] initialSlotItem;
    [Header("Drop Settings")]
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] private float dropForce = 10f;
    [SerializeField] private Vector3 dropOffset;
    [Header("Inventory Bag Settings")]
    [SerializeField] private GameObject invBagPanel;
    [SerializeField] private GameObject invBagContainer;
    [SerializeField] private GameObject interactableSlotObj;
    [Header("Block Inventory Settings")]
    public GridManager gridManager;
    private ItemSO itemData;
    private BlockSO blockData;
    private void Awake()
    {
        itemData = ItemManager.ItemData;
        blockData = BlockManager.BlockData;
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
        if(isDropable)
        {
            dropDetection();
        }
        if(haveInventoryBag)
        {
            openBagDetection();
        }
    }
    private void openBagDetection()
    {
        if(Input.GetKeyDown(invBagKey))
        {
            OpenBag();
        }
    }
    private void dropDetection()
    {
        if(Input.GetKeyDown(dropKey))
        {
            dropItem();
        }
    } 
    private void dropItem()
    {
        GameObject dropBaseObj = Instantiate(dropItemPrefab, gameObject.transform.position + dropOffset, Quaternion.identity);
        GameObject dropItem = Instantiate(itemData.item[inventory.slot[selectedSlot].getId()].itemObject,dropBaseObj.transform.position, Quaternion.identity);
        dropItem.transform.SetParent(dropBaseObj.transform);
        Collider itemCollider = dropItem.GetComponent<Collider>();
        if(itemCollider != null)
        {
            itemCollider.isTrigger = false;
        }
        dropBaseObj.layer = LayerMask.NameToLayer("DroppedItem");
        StartCoroutine(EnableCollisions(dropBaseObj, 0.5f));
        ForceDrop(dropBaseObj);
        resetSlot(selectedSlot);
        BoxCollider boxCollider = dropBaseObj.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.size = getModelSize(dropItem.transform.gameObject);
        }
    }
    private IEnumerator EnableCollisions(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);
        // Restore the collision detection by setting the layer back to the default layer
        target.layer = LayerMask.NameToLayer("Default");
    }
    private void ForceDrop(GameObject targetObj)
    {
        Rigidbody rb = targetObj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Get the player's rotation
            Quaternion playerRotation = transform.rotation;

            // Extract the forward direction from the player's rotation
            Vector3 dropDirection = playerRotation * Vector3.forward;

            // Apply the drop force to the Rigidbody
            rb.AddForce(dropDirection * dropForce, ForceMode.Impulse);
        }
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
                if (selectedSlot < inventory.GetMaxFrontSlots()- 1)
                {
                    selectedSlot++;
                } else
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
                } else
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
    public void setup()
    {
        setSlotUI(SlotPlaceHolder, "hotbar");
        setInitialInventory();
        if (invBagPanel != null)
        {
            invBagPanel.SetActive(true);
            setSlotUI(invBagContainer, "bag");
            initalizeInvBag();

            invBagPanel.SetActive(false);
        }
    }
    private void setSlotUI(GameObject slotPH, string type)
    {
        if (type == "hotbar")
        {
            for (int i = 0; i < inventory.slot.Length || i < inventory.GetMaxFrontSlots(); i++)
            {
                setSlotImg(slotPH);
            }
        }
        else if (type == "bag")
        {
            for (int i = 0; i < inventory.slot.Length; i++)
            {
                setSlotImg(slotPH);
            }
        }
    }
    private void setSlotImg(GameObject slotObj)
    {
        GameObject ui = Instantiate(slotObject, slotObj.transform.position, Quaternion.identity);
        ui.transform.SetParent(slotObj.transform);
        if (ui.GetComponent<Image>() == null)
        {
            ui.AddComponent<Image>();
        }
        Image uiImg = ui.GetComponent<Image>();
        uiImg.sprite = slotImageNormal;
        uiImg.preserveAspect = true;
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
        switch(inventoryType)
        {
            case InventoryType.item:
                if (item != -1 && item < itemData.item.Length)
                {
                    inventory.slot[id].item.SetItem(itemData.item[item].itemObject);
                }
            break;
            case InventoryType.block:
                if (item != -1 && item < blockData.blockData.Length)
                {
                    inventory.slot[id].item.SetItem(blockData.blockData[item].blockModel);

                }
            break;
        }
       
        setSlotObjUI(id, SlotPlaceHolder, slotObject);
        if (id == selectedSlot)
        {
            EquipItem(id);
        }
    }
    public void resetSlot(int id)
    {
        inventory.slot[id].item.SetItem(null);
        inventory.slot[id].setId(-1);
        inventory.slot[id].setCount(0);
        inventory.slot[id].setName("");
        Destroy(SlotPlaceHolder.transform.GetChild(id).GetChild(0).gameObject);
        ItemManager.removeChilds(placeItemLocation);
    }
    private Vector3 getModelSize(GameObject targetObject)
    {
        Vector3 size = new Vector3();
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        // Create an empty bounds to encapsulate all the renderers
        Bounds combinedBounds = new Bounds();

        // Iterate through all the renderers and expand the combined bounds
        foreach (Renderer renderer in renderers)
        {
            // Expand the combined bounds to include the renderer's bounds
            combinedBounds.Encapsulate(renderer.bounds);
            Debug.Log(renderer.bounds);
        }
        Debug.Log("Total: " + combinedBounds);
        // Get the size of the combined bounds
        size = combinedBounds.extents;
        return size;
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
            GameObject targetItem = null;
            switch (inventoryType)
            {
                case InventoryType.item:
                    targetItem = Instantiate(itemData.item[inventory.slot[slotId].getId()].itemObject, placeItemLocation.transform.position, Quaternion.identity);
                break;
                case InventoryType.block:
                    GameObject gameObject = blockData.blockData[inventory.slot[slotId].getId()].blockModel;
                    targetItem = Instantiate(gameObject, placeItemLocation.transform.position, Quaternion.identity);
                    targetItem.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    gridManager.PlaceBlockObject = gameObject;
                    gridManager.CurrentBlockId = inventory.slot[slotId].getId();
                break;
            }
            targetItem.transform.SetParent(placeItemLocation.transform);
            Collider itemCollider = targetItem.GetComponent<Collider>();
            if (itemCollider != null)
            {
                itemCollider.isTrigger = true;
            }
            targetItem.transform.rotation = new Quaternion(0,0,0,0);
        }
        Image targetImg = SlotPlaceHolder.transform.GetChild(slotId).GetComponent<Image>();
        targetImg.sprite = slotImageSelected;
        StartFadeInText(slotId);

    }
    private void setSlotObjUI(int id, GameObject slotPH, GameObject slotObj)
    {
        if (id < slotPH.transform.childCount)
        {

            GameObject targetUI = slotPH.transform.GetChild(id).gameObject;
            if (targetUI.transform.childCount > 1)
            {
                Destroy(targetUI.transform.GetChild(0).gameObject);
            }
            if (inventoryType == InventoryType.item)
            {
                if (inventory.slot[id].getId() != -1 && inventory.slot[id].getId() < itemData.item.Length)
                {
                    GameObject instantiatedUI = Instantiate(slotObj, Vector3.zero, Quaternion.identity);
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
            if(inventoryType == InventoryType.block)
            {
                if (inventory.slot[id].getId() != -1 && inventory.slot[id].getId() < blockData.blockData.Length)
                {
                    GameObject instantiatedUI = Instantiate(slotObj, Vector3.zero, Quaternion.identity);
                    instantiatedUI.transform.SetParent(targetUI.transform);
                    if (instantiatedUI.GetComponent<Image>() == null)
                    {
                        instantiatedUI.AddComponent<Image>();
                    }
                    instantiatedUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Image uiImg = instantiatedUI.GetComponent<Image>();

                    uiImg.sprite = ConvertMaterialToSprite(blockData.blockData[inventory.slot[id].getId()].blockTexture);
                    uiImg.preserveAspect = true;
                }
            }
        }

    }
    private Sprite ConvertMaterialToSprite(Material sourceMaterial)
    {
        // Get the main texture from the material
        Texture2D texture = (Texture2D)sourceMaterial.mainTexture;

        if (texture != null)
        {
            // Create a new sprite using the texture
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

            // Use the sprite as desired (e.g., assign it to a SpriteRenderer component)
            return sprite;
        }
        else
        {
            Debug.LogWarning("Texture not found in the material!");
            return null;
        }
    }
    private void initalizeInvBag()
    {
        for (int i = 0; i < inventory.slot.Length; i++)
        {
            setSlotObjUI(i, invBagContainer, interactableSlotObj);
        }
    }
    #region Text Display
    public void StartFadeInText(int itemId, Color color = default(Color))
    {
        if (color == default(Color))
        {
            color = Color.white;
        }
        // Stop any ongoing fade coroutine
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Get the item name from the item data using the provided item ID
        string itemName = "";
        if (inventory.slot[itemId].getId() != -1)
        {
            switch (inventoryType)
            {
                case InventoryType.item:
                    itemName = itemData.item[inventory.slot[itemId].getId()].itemName;
                break;

                case InventoryType.block:
                    itemName = blockData.blockData[inventory.slot[itemId].getId()].blockModel.name;
                break;
            }
        }

        // Start the fade coroutine
        fadeCoroutine = StartCoroutine(FadeText(itemName, color));
    }
    public void StartFadeInText(string text, Color color = default(Color))
    {
        if(color == default(Color))
        {
            color = Color.white;
        }
        // Stop any ongoing fade coroutine
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // Get the item name from the item data using the provided item ID
        // Start the fade coroutine
        fadeCoroutine = StartCoroutine(FadeText(text, color));
    }
    private IEnumerator FadeText(string text, Color color)
    {
        // Set the initial text and alpha value
        DisplayText.text = text;
        DisplayText.alpha = 0f;
        DisplayText.color = color;
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
    private void OpenBag()
    {
        invBagPanel.SetActive(!invBagPanel.activeInHierarchy);
        Cursor.visible = !Cursor.visible;
        if (Cursor.visible == true)
        {
            Cursor.lockState = CursorLockMode.None;
        } else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
