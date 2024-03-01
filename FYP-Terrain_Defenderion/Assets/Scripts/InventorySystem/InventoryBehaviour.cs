using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public enum InventoryType
{
    item,
    block,
    Structure
}
public class InventoryBehaviour : MonoBehaviour
{
    [Header("Toggle")]
    [SerializeField] private bool isInitializeOnStart = true;
    [SerializeField] private bool isDropable = false;
    [SerializeField] private bool haveInventoryBag = false;
    public bool HaveInventoryBag { set { haveInventoryBag = value; } }
    [Header("Input Settings")]
    public KeyCode dropKey = KeyCode.Q;
    public KeyCode invBagKey = KeyCode.E;
    public bool isItemSelectable = true;
    public bool invBagOpened { get; set; }
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
    public bool isMouseVisble = false;
    private bool preIsMouseVisible = false;
    private float scrollTimer = 0f; // Timer to track cooldown
    [Header("Main Settings")]
    [SerializeField] private InventoryType inventoryType; 
    public InventorySystem inventory;
    [SerializeField] private int selectedSlot = 1;
    public int SelectedSlot { get { return selectedSlot; } }
    [SerializeField] private GameObject slotObject;
    [SerializeField] private GameObject placeItemLocation;
    [SerializeField] private int[] initialSlotItem;
    [SerializeField] private StructureManager structureManager;
    [SerializeField] private GridGenerator gridGenerator;
    public GameObject CurrentItem 
    { 
        get 
        {
            if(placeItemLocation.transform.childCount > 0)
            {
                Debug.Log("Item: " + placeItemLocation.transform.GetChild(0).gameObject);
                return placeItemLocation.transform.GetChild(0).gameObject;
            } else
            {
                return null;
            }
        }
    }
    [Header("Custom Events")]
    [SerializeField] private UnityEvent<GameObject> CustomEquipItemEvent = new UnityEvent<GameObject>();
    [SerializeField] private UnityEvent CustomInitialEvent;
    [Header("Drop Settings")]
    [SerializeField] private GameObject dropItemPrefab;
    [SerializeField] private float dropForce = 10f;
    [SerializeField] private Vector3 dropOffset;
    [Header("Inventory Bag Settings")]
    [SerializeField] private InventoryMenuManager inventoryMenuManager;
    [SerializeField] private InventoryBagSO invBagSO;
    [SerializeField] private GameObject slotBasePrefab;
    [SerializeField] private GameObject invBagPanel;
    [SerializeField] private GameObject menuTypeObj;
    [SerializeField] private GameObject menuContent;
    [SerializeField] private float menuContentOffset = 25f;
    [SerializeField] private GameObject menuContentPrefab;
    [SerializeField] private GameObject menuScrollView;
    [SerializeField] private int menuPage = 0;
    [SerializeField] private bool useCustomOpenInvBagEvent = false;
    [SerializeField] private UnityEvent customOpenInvBagEvent;
    [Header("Block Inventory Settings")]
    public GridManager gridManager;
    private ItemSO itemData;
    private BlockSO blockData;
    [Header("Capture Settings")]
    [SerializeField] private Camera captureCamera;
    [SerializeField] private GameObject captureContainer;
    [SerializeField] private string captureSavePath = "/StructureData/Inventory/Temp";
    private GameObject ToolTipGameobject;
    private void Awake()
    {
        StartCursorState();
        itemData = ItemManager.ItemData;
        blockData = BlockManager.BlockData;
    }
    private void OnValidate()
    {
        if(isMouseVisble != preIsMouseVisible)
        {
            StartCursorState();
            preIsMouseVisible = isMouseVisble;
        }
    }
    public void StartCursorState()
    {
        StartCoroutine(CursorState());
    }
    public void ToggleCursorState(bool state = default)
    {
        if (state == default)
        {
            isMouseVisble = !isMouseVisble;
        } else
        {
            isMouseVisble = state;
        }
        StartCursorState();
    }
    private IEnumerator CursorState()
    {
        yield return null;
        FlightController flightController = GetComponent<FlightController>();
        #region MouseVisible State
        if (isMouseVisble)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if(flightController)
            {
                flightController.enabled = false;
            }
            isItemSelectable = false;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            if (flightController)
            {
                flightController.enabled = true;
            }
            isItemSelectable = true;
        }
        yield break;
        #endregion
    }
    // Start is called before the first frame update
    void Start()
    {
        #region ShowItem(Debug)
        //Debug.Log("Item List: ");
        //for (int i = 0; i < itemData.item.Length; i++)
        //{
        //    Debug.Log(itemData.item[i].itemObject);
        //}
        #endregion
        if (inventory == null)
        {
            inventory = gameObject.GetComponent<InventorySystem>();
        }
        if(isInitializeOnStart)
        setup();
        ToolTipGameobject = GameObject.FindGameObjectWithTag("ToolTip");
    }
    public void SetupInventory()
    {
        setup();
        isInitializeOnStart = true;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (isInitializeOnStart)
        {
            if (isItemSelectable)
            {
                inputNumDetection();
            }
            if (isDropable)
            {
                dropDetection();
            }
            if (haveInventoryBag)
            {
                openBagDetection();
            }
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
        if(haveInventoryBag)
        InitialInvBag();
        CustomInitialEvent.Invoke();
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
        if(inventoryType == InventoryType.item)
        {
            Destroy(ui.GetComponent<BoxCollider>());
        }
        if (inventoryType == InventoryType.block || inventoryType == InventoryType.Structure)
        {
            Button button = ui.AddComponent<Button>();
            button.targetGraphic = uiImg;
            ColorBlock colorBlock = button.colors;
            colorBlock.highlightedColor = Color.green;
            button.colors = colorBlock;
            SlotBehaviour slotBehaviour = ui.AddComponent<SlotBehaviour>();
            slotBehaviour.Initialize(this, slotObj.transform.childCount - 1, SlotType.InventorySlot);
        }

    }
    private void setInitialInventory()
    {
        for (int i = 0; i < initialSlotItem.Length; i++)
        {
            setSlotItem(i, initialSlotItem[i]);
        }
    }
    public void setSlotItem(int id, int item, bool equipItem = true)
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
            case InventoryType.Structure:
 
                break;
        }
       
        setSlotObjUI(id, SlotPlaceHolder, slotObject, item);
        if (id == selectedSlot)
        {
            if(equipItem)
            EquipItem(id);
        }
    }
    public void UpdateSlotDisplay(string text)
    {
        if(inventory.slot[selectedSlot].ItemDisplay != null)
            inventory.slot[selectedSlot].ItemDisplay.text = text;
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
        if (inventory.slot[slotId].getId() >= -1 && slotId < inventory.slot.Length)
        {

            GameObject targetItem = null;
            int invId = inventory.slot[slotId].getId();
            switch (inventoryType)
            {
                case InventoryType.item:
                    if (invId >= 0)
                    {
                        targetItem = Instantiate(itemData.item[invId].itemObject, placeItemLocation.transform.position, Quaternion.identity);
                    }
                     break;
                case InventoryType.block:
                    if(invId == -1)
                    {
                        gridManager.PlaceBlockObject = null;
                        gridManager.CurrentBlockId = -1;
                        break;
                    }
                    if (blockData.blockData[invId].isUtility == false)
                    {
                        GameObject gameObject = blockData.blockData[invId].blockModel;
                        targetItem = Instantiate(gameObject, placeItemLocation.transform.position, Quaternion.identity);
                        targetItem.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        gridManager.PlaceBlockObject = gameObject;
                    }
                    if(blockData.blockData[invId].isUtility == true)
                    {
                        if (blockData.blockData[invId].blockModel != null)
                        {
                            GameObject gameObject = blockData.blockData[invId].blockModel;
                            targetItem = Instantiate(gameObject, placeItemLocation.transform.position, Quaternion.identity);
                        }
                        else
                        {
                            targetItem = GameObject.CreatePrimitive(PrimitiveType.Cube);
                            targetItem.transform.position = placeItemLocation.transform.position;
                            targetItem.transform.rotation = Quaternion.identity;

                            Renderer targetRender = targetItem.GetComponent<Renderer>();
                            targetRender.material = blockData.blockData[invId].blockTexture;
                        }
                        targetItem.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                        gridManager.PlaceBlockObject = null;
                    }
                    gridManager.CurrentBlockId = inventory.slot[slotId].getId();
                     break;
                case InventoryType.Structure:
                    structureManager.CurrentStructure = inventory.slot[slotId].getId();
                    break;
            }
            if (targetItem != null)
            {
                targetItem.transform.SetParent(placeItemLocation.transform);
                if (inventoryType == InventoryType.item)
                {
                    targetItem.transform.localPosition = itemData.item[invId].itemObject.transform.position;
                    targetItem.transform.localRotation = itemData.item[invId].itemObject.transform.rotation;
                    //targetItem.transform.position += itemData.item[invId].itemObject.transform.position;
                } else
                {
                    targetItem.transform.rotation = new Quaternion(0, 0, 0, 0);
                }
                Collider itemCollider = targetItem.GetComponent<Collider>();
                if (itemCollider != null)
                {
                    itemCollider.isTrigger = true;
                }
            }
            CustomEquipItemEvent.Invoke(targetItem);
        }
        Image targetImg = SlotPlaceHolder.transform.GetChild(slotId).GetComponent<Image>();
        targetImg.sprite = slotImageSelected;
        StartFadeInText(slotId);
        
    }

    private void setSlotObjUI(int id, GameObject slotPH, GameObject slotObj, int item = -1)
    {
        if (id < slotPH.transform.childCount)
        {

            GameObject targetUI = slotPH.transform.GetChild(id).gameObject;
            GameObjectExtension.RemoveAllObjectsFromParent(targetUI.transform);
            if (inventoryType == InventoryType.item)
            {
                if (inventory.slot[id].getId() != -1 && inventory.slot[id].getId() < itemData.item.Length)
                {
                    GameObject instantiatedUI = Instantiate(slotObj, Vector3.zero, Quaternion.identity);
                    instantiatedUI.transform.SetParent(targetUI.transform);
                    TMP_Text displayText = instantiatedUI.transform.GetChild(0).GetComponent<TMP_Text>();
                    inventory.slot[id].ItemDisplay = displayText;
                    if (instantiatedUI.GetComponent<Image>() == null)
                    {
                        instantiatedUI.AddComponent<Image>();
                    }
                    instantiatedUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    Image uiImg = instantiatedUI.GetComponent<Image>();
                    RectTransform rectTransform = instantiatedUI.GetComponent<RectTransform>();
                   
                    if (itemData.item[inventory.slot[id].getId()].itemObject != null )
                    {
                        //Capture Item and display on Slot UI
                        float width = rectTransform.rect.width * 2f;
                        float height = rectTransform.rect.height * 2f;
                        rectTransform.sizeDelta = new Vector2(width, height);
                        GameObject captureItem = Instantiate(itemData.item[inventory.slot[id].getId()].itemObject);
                        float cameraSize = captureCamera.orthographicSize;
                        captureItem.transform.SetParent(captureCamera.transform);
                        captureItem.transform.eulerAngles =new Vector3(313.390015f, 236.100006f, 0f);
                        captureItem.transform.localPosition = Vector3.forward * 10 - new Vector3(0f,0.05f,0f);
                        if (displayText != null)
                        {
                            //Gun Specific Setup
                            GunController gunController = captureItem.GetComponent<GunController>();
                            if (gunController != null)
                            {
                                displayText.text = gunController.GetRemainAmmo() + "/" + gunController.GetTotalAmmo();
                            }
                        }

                        ModelPictureSaver.CaptureAndSaveImage(captureCamera, captureItem, captureSavePath, id.ToString(), true, true, itemData.item[inventory.slot[id].getId()].CustomBuffer);

                        uiImg.sprite = StructureSerializer.LoadSpriteFromFile(captureSavePath + "/" + id + ".png");
                        uiImg.raycastTarget = false;
                        captureCamera.orthographicSize = cameraSize;
                    }
                    if(itemData.item[inventory.slot[id].getId()].useItemSprite)
                    {
                        uiImg.sprite = itemData.item[inventory.slot[id].getId()].itemSprite;
                    }
                    uiImg.preserveAspect = true;
                    uiImg.raycastTarget = false;
                    SimpleTooltip simpleTooltip = SlotPlaceHolder.GetComponent<SimpleTooltip>();
                    if(simpleTooltip!=null)
                    simpleTooltip.infoLeft = itemData.item[inventory.slot[id].getId()].itemName;
                }
            }
            if(inventoryType == InventoryType.block)
            {
                if (inventory.slot[id].getId() != -1 && inventory.slot[id].getId() < blockData.blockData.Length)
                {
                   
                    int invId = inventory.slot[id].getId();
                    GameObject instantiatedUI = Instantiate(slotObj, Vector3.zero, Quaternion.identity);
                    TMP_Text displayText = instantiatedUI.transform.GetChild(0).GetComponent<TMP_Text>();
                    instantiatedUI.transform.SetParent(targetUI.transform);
                    if (instantiatedUI.GetComponent<Image>() == null)
                    {
                        instantiatedUI.AddComponent<Image>();
                    }
                    instantiatedUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    RectTransform rectTransform = instantiatedUI.GetComponent<RectTransform>();
                    Image uiImg = instantiatedUI.GetComponent<Image>();
                    if (blockData.blockData[inventory.slot[id].getId()].blockModel != null)
                    {
                        float width = rectTransform.rect.width * 2.5f;
                        float height = rectTransform.rect.height * 2.5f;
                        rectTransform.sizeDelta = new Vector2(width, height);
                        GameObject gameObject = Instantiate(blockData.blockData[invId].blockModel);
                        float cameraSize = gridManager.captureCamera.orthographicSize;
                        gameObject.transform.SetParent(gridManager.captureCamera.transform);
                        gameObject.transform.localPosition = Vector3.forward * 10;
                        gridManager.captureCamera.orthographicSize = blockData.blockData[invId].captureOrthographicSize;
                        ModelPictureSaver.CaptureAndSaveImage(gridManager.captureCamera, gameObject, captureSavePath, id.ToString(), true, true, 0.5f);

                        uiImg.sprite = StructureSerializer.LoadSpriteFromFile(captureSavePath + "/" + id + ".png");
                        uiImg.raycastTarget = false;
                        gridManager.captureCamera.orthographicSize = cameraSize;
                        if (item != -1 )
                        {
                            displayText.text = "Cost: " + blockData.blockData[invId].tokenCost;
                        }
                    } else
                    {
                        float width = rectTransform.rect.width * 1.5f;
                        float height = rectTransform.rect.height * 1.5f;
                        rectTransform.sizeDelta = new Vector2(width, height);
                        uiImg.sprite = ConvertMaterialToSprite(blockData.blockData[invId].blockTexture);
                    }
                    uiImg.preserveAspect = true;
                    SimpleTooltip simpleTooltip = SlotPlaceHolder.GetComponent<SimpleTooltip>();
                    if (simpleTooltip != null)
                        simpleTooltip.infoLeft = blockData.blockData[inventory.slot[id].getId()].blockModel.name;
                }
            }
            if(inventoryType == InventoryType.Structure)
            {
                int invId = inventory.slot[id].getId();
                GameObject instantiatedUI = Instantiate(slotObj, targetUI.transform.position, Quaternion.identity, targetUI.transform);
                Image image = instantiatedUI.GetComponent<Image>();
                TMP_Text displayText = instantiatedUI.transform.GetChild(0).GetComponent<TMP_Text>();
                if (image == null)
                {
                    image = instantiatedUI.AddComponent<Image>();
                }
                image.preserveAspect = true;
                image.raycastTarget = false;
                RectTransform rectTransform = instantiatedUI.GetComponent<RectTransform>();
                if (invId != -1)
                {
                    if (structureManager.structurePoolings[invId].structures != null && structureManager.structurePoolings[invId].structures.Count > 0)
                    {
                        image.sprite = structureManager.structurePoolings[invId].structureImage;
                        InventoryBehaviour.SetRectTransformFromStructureSize(rectTransform, structureManager.structurePoolings[invId].structureSize);
                        SimpleTooltip simpleTooltip = SlotPlaceHolder.GetComponent<SimpleTooltip>();
                        if (simpleTooltip != null)
                            simpleTooltip.infoLeft = structureManager.structurePoolings[invId].name;
                        if (item != -1)
                        {
                            displayText.text = "Cost: " + structureManager.structurePoolings[invId].structures[0].GetComponent<GridData>().tokenCost;
                        }
                    }
                } else
                {

                        displayText.text = "";
                }
                if(image.sprite == null)
                {
                    image.color = Color.clear;
                }

            }
        }

    }
    private void OnDisable()
    {
        if(gameObject.GetComponent<HealthBehaviour>() != null && CurrentItem != null && CurrentItem.GetComponent<GunController>()!=null)
        {
            CurrentItem.GetComponent<GunController>().SetActiveState(false);
        }
    }
    private void SetBlockSlotUI(int id, GameObject slotObj, Transform parentObj)
    {
        
        int invId = id;
        GameObject instantiatedUI = Instantiate(slotObj, Vector3.zero, Quaternion.identity, parentObj);
        if (instantiatedUI.GetComponent<Image>() == null)
        {
            instantiatedUI.AddComponent<Image>();
        }
        instantiatedUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        RectTransform rectTransform = instantiatedUI.GetComponent<RectTransform>();
        Image uiImg = instantiatedUI.GetComponent<Image>();
        uiImg.raycastTarget = false;
        if (blockData.blockData[invId].blockModel != null)
        {
            float width = rectTransform.rect.width * 2.5f;
            float height = rectTransform.rect.height * 2.5f;
            rectTransform.sizeDelta = new Vector2(width, height);
            GameObject gameObject = Instantiate(blockData.blockData[invId].blockModel);
            float cameraSize = gridManager.captureCamera.orthographicSize;
            gameObject.transform.SetParent(gridManager.captureCamera.transform);
            gameObject.transform.localPosition = Vector3.forward * 10;
            gridManager.captureCamera.orthographicSize = blockData.blockData[invId].captureOrthographicSize;
            ModelPictureSaver.CaptureAndSaveImage(gridManager.captureCamera, gameObject, captureSavePath, id.ToString(), true, true, 0.5f);

            uiImg.sprite = StructureSerializer.LoadSpriteFromFile(captureSavePath + "/" + id + ".png");
            gridManager.captureCamera.orthographicSize = cameraSize;
            if(instantiatedUI.transform.childCount > 0)
            {
                Destroy(instantiatedUI.transform.GetChild(0).gameObject);
            }
        }
        else
        {
            float width = rectTransform.rect.width * 1.5f;
            float height = rectTransform.rect.height * 1.5f;
            rectTransform.sizeDelta = new Vector2(width, height);
            uiImg.sprite = ConvertMaterialToSprite(blockData.blockData[invId].blockTexture);
        }
        uiImg.preserveAspect = true;
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
    //private void initalizeInvBag()
    //{
    //    for (int i = 0; i < inventory.slot.Length; i++)
    //    {
    //        setSlotObjUI(i, invBagContainer, interactableSlotObj);
    //    }
    //}
    #region Text Display
    public void StartFadeInText(int itemId, Color color = default(Color))
    {
        int invId = inventory.slot[itemId].getId();
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
        if (invId != -1)
        {
            switch (inventoryType)
            {
                case InventoryType.item:
                    itemName = itemData.item[invId].itemName;
                break;

                case InventoryType.block:
                    if (blockData.blockData[invId].blockModel != null)
                        itemName = blockData.blockData[invId].blockModel.name;
                    else
                    {
                        itemName = blockData.blockData[invId].blockTexture.name;
                    }
                break;
                case InventoryType.Structure:
                    itemName = structureManager.structurePoolings[invId].name;
                    break;
            }
        }

        // Start the fade coroutine
        fadeCoroutine = StartCoroutine(FadeText(itemName, color));
    }
    public void StartFadeInText(string text, Color color = default(Color), float duration = default)
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
        fadeCoroutine = StartCoroutine(FadeText(text, color, duration));
    }
    private IEnumerator FadeText(string text, Color color, float duration = default)
    {
        if(duration == default)
        {
            duration = displayDuration;
        }
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
        yield return new WaitForSeconds(duration);

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
    private void InitialInvBag()
    {
        if (invBagPanel != null)
        {
            invBagPanel.SetActive(true);
            for (int i = 0; i < invBagSO.inventoryBag.Length; i++)
            {
                InvBagSetup(i, false);
            }
            SetMenuPage(menuPage);
            InvBagTypeSetup();
            invBagPanel.SetActive(false);
        }
    }
    public void SetMenuPage(int page)
    {
        menuContent.SetActive(false);
        menuContent = menuScrollView.transform.GetChild(0).GetChild(page).gameObject;
        menuContent.SetActive(true);
        ScrollRect scrollRect = menuScrollView.GetComponent<ScrollRect>();
        scrollRect.content = menuContent.GetComponent<RectTransform>();
    }
    private void InvBagTypeSetup()
    {
        if (invBagSO == null) return;
        GameObjectExtension.RemoveAllObjectsFromParent(menuTypeObj.transform);
        //Type Button generation

        for (int i = 0; i < invBagSO.inventoryBag.Length; i++)
        {
            GameObject typeBtn = Instantiate(invBagSO.typeBtnPrefab, menuTypeObj.transform.position, Quaternion.identity, menuTypeObj.transform);
            TMP_Text text = GameObjectExtension.GetGameObjectWithTagFromChilds(typeBtn, "Text").GetComponent<TMP_Text>();
            text.text = invBagSO.inventoryBag[i].TypeName;
            InventoryMenuTypeBehaviour inventoryMenuTypeBehaviour = typeBtn.GetComponent<InventoryMenuTypeBehaviour>();
            inventoryMenuTypeBehaviour.Initialize(i, this, inventoryMenuManager);
            inventoryMenuManager.AddInvMenu(-1, typeBtn.GetComponent<Image>());
        }
    }
    private void InvBagSetup(int page, bool setPage = true)
    {
        if (invBagSO == null) return;
        if(menuPage != page)
        {
            if(setPage)
            menuPage = page;
        }
        menuContent = Instantiate(menuContentPrefab, menuScrollView.transform.GetChild(0).position, Quaternion.identity, menuScrollView.transform.GetChild(0));
        float mainHeight = 0f;
        //Menu Item generation
        RectTransform mainRect = menuContent.GetComponent<RectTransform>();
        foreach (InvMenu invMenu in invBagSO.inventoryBag[page].invMenus)
        {
            GameObject menuNameObj = Instantiate(invBagSO.MenuNamePrefab, menuContent.transform.position, Quaternion.identity, menuContent.transform);
           
            TMP_Text text1 = GameObjectExtension.GetGameObjectWithTagFromChilds(menuNameObj, "Text").GetComponent<TMP_Text>();
            text1.text = invMenu.MenuName;
            mainHeight += menuNameObj.GetComponent<RectTransform>().rect.height;
            GameObject menuSlot = Instantiate(invBagSO.MenuSlotPrefab, menuContent.transform.position, Quaternion.identity, menuContent.transform);
            foreach(int itemID in invMenu.BlockID)
            {
                GameObject slotItem = Instantiate(slotBasePrefab, menuSlot.transform.position, Quaternion.identity, menuSlot.transform);
                SimpleTooltip simpleTooltip = slotItem.GetComponent<SimpleTooltip>();
                SlotBehaviour slotBehaviour = slotItem.GetComponent<SlotBehaviour>();
                if (inventoryType == InventoryType.block)
                {
                    SetBlockSlotUI(itemID, slotObject, slotItem.transform);
                    simpleTooltip.infoLeft = "&" +blockData.blockData[itemID].blockModel.name + "\n\n`Stats:" + "\n$Type: " + invMenu.MenuName  + "\n~Health: " + blockData.blockData[itemID].maxHealth + "\n!Token Cost: " + blockData.blockData[itemID].tokenCost + "\n\n`-Click to select-";
                    slotBehaviour.Initialize(this, itemID, SlotType.InventoryBag);
                }
                if(inventoryType == InventoryType.Structure)
                {
                    SetStructureSlotUI(itemID, slotObject, slotItem.transform);
                    simpleTooltip.infoLeft = "&" + structureManager.structurePoolings[itemID].name + "\n\n`Stats:" + "\n$Type: " + invMenu.MenuName + "\n~Health: " + structureManager.structurePoolings[itemID].structures[0].GetComponent<HealthBehaviour>().GetHealth() + "\n!Token Cost: " + structureManager.structurePoolings[itemID].structures[0].GetComponent<GridData>().tokenCost + "\n`-Click to select-";
                    slotBehaviour.Initialize(this, itemID, SlotType.StructureEditor);
                }
            }
            RectTransform rectTransform = menuSlot.GetComponent<RectTransform>();
            Debug.Log(rectTransform.rect.width);
            float height = slotBasePrefab.GetComponent<RectTransform>().rect.height * Mathf.Ceil((menuSlot.transform.childCount * menuSlot.GetComponent<GridLayoutGroup>().cellSize.x)/inventoryMenuManager.GetComponent<RectTransform>().rect.width ) + menuContentOffset;
            mainHeight += height;
            rectTransform.sizeDelta = new Vector2(rectTransform.rect.width, height);
        }
        
        mainRect.sizeDelta = new Vector2(mainRect.rect.x, mainHeight);
        Vector3 pos = mainRect.position;
        pos.x = 0f;
        mainRect.anchoredPosition = pos;
        menuContent.SetActive(false);
        
    }
    private void SetSlotsToolTip( bool state)
    {
        foreach(Transform tooltip in menuContent.transform)
        {
            SimpleTooltip ToolTip = tooltip.GetComponent<SimpleTooltip>();
            ToolTip.enabled = state;
        }
    }
    private void OpenBag()
    {
        if (gridManager != null && gridManager.IsMenuOpen) return;
        invBagOpened = !invBagOpened;
        if (gridManager != null)
        {
            gridManager.Gridgenerator.BuildMode = !invBagOpened;
        }
        if (gridGenerator != null)
        {
            gridGenerator.BuildMode = !invBagOpened;
        }
        if (!useCustomOpenInvBagEvent)
        {
            invBagPanel.SetActive(invBagOpened);
            ToolTipGameobject.SetActive(invBagOpened);
            ToolTipGameobject.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
            ToggleCursorState(invBagOpened);
        } else
        {
            customOpenInvBagEvent.Invoke();
            ToolTipGameobject.SetActive(invBagOpened);
            ToolTipGameobject.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "";
            ToggleCursorState(invBagOpened);
        }
       
    }
    #region structure
    private void SetStructureSlotUI(int id, GameObject slotObj, Transform parent)
    {
        int invId = id;
        GameObject instantiatedUI = Instantiate(slotObj, Vector3.zero, Quaternion.identity, parent);
        if (instantiatedUI.GetComponent<Image>() == null)
        {
            instantiatedUI.AddComponent<Image>();
        }
        instantiatedUI.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        RectTransform rectTransform = instantiatedUI.GetComponent<RectTransform>();
        Image uiImg = instantiatedUI.GetComponent<Image>();
        if (structureManager != null)
        {

            uiImg.sprite = structureManager.structurePoolings[invId].structureImage;
            InventoryBehaviour.SetRectTransformFromStructureSize(rectTransform, structureManager.structurePoolings[invId].structureSize);
        }
        if (instantiatedUI.transform.childCount > 0)
        {
            Destroy(instantiatedUI.transform.GetChild(0).gameObject);
        }
        uiImg.preserveAspect = true;
        uiImg.raycastTarget = false;
    }
    public static void SetRectTransformFromStructureSize(RectTransform rectTransform, GridSize gridSize)
    {
        float width = 0f;
        float height = 0f;
        Vector2 newPosition = rectTransform.anchoredPosition;

        switch (gridSize)
        {
            case GridSize.Small:
                width = rectTransform.rect.width * 2.5f;
                height = rectTransform.rect.height * 2.5f;
                break;
            case GridSize.Normal:
                width = rectTransform.rect.width * 2.5f;
                height = rectTransform.rect.height * 2.5f;
                break;
            case GridSize.Large:
                width = rectTransform.rect.width * 2.25f;
                height = rectTransform.rect.height * 2.5f;
                break;
        }
        rectTransform.sizeDelta = new Vector2(width, height);
        rectTransform.anchoredPosition = newPosition;
    }
    #endregion
}
