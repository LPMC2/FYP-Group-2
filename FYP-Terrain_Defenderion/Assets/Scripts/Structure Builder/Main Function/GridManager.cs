using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System.Linq;
using NaughtyAttributes;
public class GridManager : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private GridUIManager gridUIManager;
    [SerializeField] private GridInputManager gridInputManager;
    [SerializeField] private LayerMask GridLayer;
    [Header("Input Settings")]
    [SerializeField] private KeyCode openMenuKey = KeyCode.M;
    [Header("User Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject cameraObject;
    [Header("Display Settings")]
    [SerializeField] private GameObject displayActionBar;
    [SerializeField] private GameObject gridPlane;
    public Camera captureCamera;
    [Header("Token Settings")]
    [SerializeField] private GameObject tokenAmountTextDisplay;
    [Header("Menu Settings")]
    private bool isMenuOpen = false;
    [SerializeField] private Image tempStructureImg;
    [SerializeField] private string tempImgPath = "/StructureData/Temp/tempStructureImg.png";
    [SerializeField] private TMP_Text structureNameText;
    [SerializeField] private Image structureImg;
    [SerializeField] private GameObject saveMenu;
    [SerializeField] private GameObject loadMenu;
    [Header("Grid Settings")]
    public int numRows = 5;
    public int numColumns = 5;
    public int numHeight = 5;
    public float cellSize = 1f;
    [SerializeField] private int m_MaxDefenseCount = 10;
    private int defenseCount = 0;
    public int MaxDefenseCount { get { return m_MaxDefenseCount; } }
    public GameObject gridContainer;
    [SerializeField] private GameObject gridBlockObj;
    [SerializeField] private GameObject gridVisualLayer;
    [SerializeField] private GameObject gridVisibleObj;
    [SerializeField] private float ContactRange = 10f;
    [SerializeField] private GameObject placeBlockObject;
    [SerializeField] private int currentBlockId;
    [Header("Menu & Saving Settings")]
    [SerializeField] private GameObject menuObj;
    #region placeBlockObject + currentBlockId Setter and Getter
    public GameObject PlaceBlockObject {
        get { return placeBlockObject; }
        set { placeBlockObject = value; }
    }
    public int CurrentBlockId
    {
        get { return currentBlockId; }
        set { currentBlockId = value; }
    }
    #endregion
    [SerializeField] private string path = "Default";
    string imgPath = "Default";
    private int structureId = 0;
    [SerializeField]private string name = "";
    #region pathSetter
    public void SetPath(string str)
    {
        path = "/StructureData/StructureFile/" + str + ".json";
        name = str;
    }
    #endregion
    [Header("Place/Break Settings")]
    [SerializeField] private bool isEditable = true;
    #region isEditable Toggler
    public void ToggleIsEditable(bool state = default)
    {
        if (state == default)
        {
            isEditable = !isEditable;
            isMenuOpen = !isEditable;
        } else
        {
            isEditable = state;
            isMenuOpen = state;
        }
    }
    #endregion
    private bool canPlace = true; // Flag to track if placing is allowed
    private bool canBreak = true; // Flag to track if breaking is allowed
    [SerializeField] private float placeCooldown = 0.05f; // Cooldown duration for placing (in seconds)
    [SerializeField] private float breakCooldown = 0.05f; // Cooldown duration for breaking (in seconds)
    public float BreakCoolDown { get {return breakCooldown; } }
    private float placeTimer = 0.0f; // Timer for placing cooldown
    private float breakTimer = 0.0f; // Timer for breaking cooldown
    private GridCell[,] grid;
    private GameObject[,] cellVisuals;
    private GameObject lastOutlineBlock;
    private List<GameObject> hitList = new List<GameObject>();
    private BlockSO blockData;
    private TokenManager tokenManager;
    [SerializeField] private GameObject currentHitObject;
    #region Menu UI
    public void SetSaveMenuImg()
    {
        Sprite sprite = StructureSerializer.LoadSpriteFromFile(tempImgPath);
        tempStructureImg.sprite = sprite;
    }
    #endregion

    #region Menu Keycode Detection
    private void OnEnable()
    {
        StartCoroutine(DetectKeyCode());
    }

    private void OnDisable()
    {
        StopCoroutine(DetectKeyCode());
    }

    private IEnumerator DetectKeyCode()
    {
        while (true)
        {
            if (Input.GetKeyDown(openMenuKey))
            {
                if (saveMenu.activeInHierarchy == false && loadMenu.activeInHierarchy == false)
                {
                    if (menuObj != null)
                    {

                        ToggleEditState();

                    }
                }
            }

            yield return null;
        }
    }
    public void ToggleEditState(bool state = default)
    {
        Animator animator = menuObj.GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("State");
        InventoryBehaviour inventoryBehaviour = player.GetComponent<InventoryBehaviour>();
        if (inventoryBehaviour != null)
        {
            ToggleIsEditable(state);
            inventoryBehaviour.ToggleCursorState(state);
        }
    }
    #endregion

    #region Selction Function
    public void SelectStructure(int value = 0)
    {
        // -1 -> Left, 1 -> Right, 0 -> Unset
        value = Mathf.Clamp(value, -1, 1);
        structureId += value;
        //Debug.Log("Length " + (StructureSerializer.GetFileItemsLength("/StructureData/StructureFile/") - 1));
        structureId = Mathf.Clamp(structureId, 0, StructureSerializer.GetFileItemsLength("/StructureData/StructureFile/") -1);
        SetStructurePath(structureId);
        SetStructureImg(structureId);
       
    }
    public void SetSelection(int value)
    {
        structureId = value;
    }
    private void SetStructurePath(int id)
    {
        StructureTypeFile structureTypeFile = StructureSerializer.SearchStructureFile(id, StructureSerializer.StructureType.file);
        path = structureTypeFile.FilePath;
        structureNameText.text = structureTypeFile.FileName;
    }
    private void SetStructureImg(int id)
    {
        StructureTypeFile structureTypeFile = StructureSerializer.SearchStructureFile(id, StructureSerializer.StructureType.image);
        if (structureTypeFile.ImageData != null)
        {
            structureImg.sprite = structureTypeFile.ImageData;
        }
    }
    #endregion
    private void Awake()
    {
        blockData = BlockManager.BlockData;
        gridInputManager = gameObject.GetComponent<GridInputManager>();

        gridUIManager = gameObject.GetComponent<GridUIManager>();
    }
    private void Start()
    {
        
        gridGenerator.SetGridGenerator(this,blockData, currentBlockId, numRows, numColumns, numHeight, cellSize, gridContainer.transform, cameraObject.transform, ContactRange);
        tokenManager = player.GetComponent<TokenManager>();
        if (tokenManager != null)
        {
            UpdateTokenDisplay(tokenManager.getTokens());
        }
        if (isEditable)
        {
            CreateGrid();
            CreateVisualGrid();
            //CreateInteractableGrid();
        }
    }
    T GetSecondToLastItem<T>(List<T> list)
    {
        if (list.Count >= 2)
        {
            return list[list.Count - 2];
        }

        // Handle cases where the list doesn't have enough items
        // For example, you can throw an exception or return a default value
        throw new InvalidOperationException("Not enough items in the list.");
    }
    private void CreateGrid()
    {
        grid = new GridCell[numRows, numColumns];

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                grid[row, col] = new GridCell(row, col);
            }
        }
    }
    private void CreateVisualGrid()
    {
        RectTransform rectTransform = gridVisibleObj.GetComponent<RectTransform>();
        SpriteRenderer spriteRenderer = gridVisibleObj.GetComponent<SpriteRenderer>();
        rectTransform.localScale = new Vector3(cellSize,cellSize, cellSize);
        spriteRenderer.size = new Vector2(numRows, numColumns);
        gridPlane.transform.localScale = new Vector3(numRows / 10f, 1f, numColumns / 10f);
        //for(int row = 0; row < numRows; row++)
        //{
        //    for (int col = 0; col < numColumns; col++)
        //    {
        //        GameObject cellVisual = GameObject.Instantiate(gridVisibleObj, gridVisualLayer.transform.position, Quaternion.identity);
        //        cellVisual.transform.SetParent(gridVisualLayer.transform);
        //        cellVisual.transform.rotation = new Quaternion(0f,0f,0f,0f);
        //    }
        //}
    }
    public void CreateInteractableGrid()
    {
        cellVisuals = new GameObject[numRows, numColumns];
        int count = 0;
        // Calculate the offset to position the grid at the center of the current holder object
        Vector3 gridOffset = new Vector3((numRows - 1) * cellSize * 0.5f, -(cellSize / 2), (numColumns - 1) * cellSize * 0.5f);

        for (int height = 0; height < numHeight; height++)
        {
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numColumns; col++)
                {
                    GameObject cellInteractable = GameObject.Instantiate(gridBlockObj, gridContainer.transform.position + new Vector3(row * cellSize, height * cellSize, col * cellSize) - gridOffset, Quaternion.identity);
                    cellInteractable.name = "GridBlock: " + (row + col + height);
                    GridData gridData = cellInteractable.AddComponent<GridData>();
                    gridData.cellX = row;
                    gridData.cellY = col;
                    gridData.cellHeight = height;
                    cellInteractable.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                    cellInteractable.transform.SetParent(gridContainer.transform);
                    cellVisuals[row, col] = cellInteractable;

                    if (height > 0 && cellInteractable != null)
                    {
                        cellInteractable.SetActive(false);
                    }
                    gridObjects.Add(cellInteractable);
                    count++;
                }
            }
        }
    }

    private void Update()
    {
        if(!isEditable)
        {
            return;
        }
        if (!canPlace)
        {
            placeTimer += Time.deltaTime;
            if (placeTimer >= placeCooldown)
            {
                canPlace = true;
                placeTimer = 0.0f;
            }
        }

        if (!canBreak)
        {
            breakTimer += Time.deltaTime;
            if (breakTimer >= breakCooldown)
            {
                canBreak = true;
                breakTimer = 0.0f;
            }
        }
        PerformOutlineRaycast();
        if (Input.GetMouseButtonDown(1) && canPlace)
        {
            //gridGenerator.BuildBlock(placeBlockObject, gridContainer.transform, cameraObject.transform, ContactRange);
            //PerformPlaceRaycast();
            canPlace = false;
        }

        if (Input.GetMouseButtonDown(0) && canBreak)
        {
            //gridGenerator.DestroyBlock(cameraObject.transform, ContactRange);
            //PerformBreakRaycast();
            canBreak = false;
        }
        //UpdateLoadGrid();
    }
    List<GameObject> gridObjects = new List<GameObject>();
    private void UpdateLoadGrid()
    {

        foreach (Transform target in gridContainer.transform)
        {
            float distance = Vector3.Distance(cameraObject.transform.position, target.position);
            if (distance <= ContactRange && !target.gameObject.activeInHierarchy)
            {
                target.gameObject.SetActive(true);
                
            }
            else if (distance > ContactRange && target.childCount == 0 && target.gameObject.activeInHierarchy)
            {
                target.gameObject.SetActive(false);
            }
        }
    }
    private void PerformPlaceRaycast()
    {
        RaycastHit[] hits = new RaycastHit[(int)ContactRange];
        Ray ray = new Ray(cameraObject.transform.position, cameraObject.transform.forward);
        hits = Physics.RaycastAll(ray, ContactRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        Physics.RaycastNonAlloc(ray, hits);
        HandlePlaceBlock(hits, ray);
        Debug.DrawRay(cameraObject.transform.position, cameraObject.transform.forward * ContactRange, Color.red, 5f);

    }

    private void PerformOutlineRaycast()
    {
        List<RaycastHit> hits = new List<RaycastHit>((int)(ContactRange * ContactRange));
        Ray ray = new Ray(cameraObject.transform.position, cameraObject.transform.forward);
        RaycastHit[] hitsArray = new RaycastHit[(int)(ContactRange * ContactRange)];

        Physics.RaycastNonAlloc(ray, hitsArray,ContactRange * ContactRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        hits.AddRange(hitsArray);
        GameObject newHitBlock = null;

        foreach (var hit in hits)
        {
            if(hit.collider != null)
            if (hit.collider.isTrigger && hit.collider.CompareTag("Grid") && hit.collider.transform.childCount > 0)
            {
                newHitBlock = hit.collider.transform.GetChild(0).gameObject;
                break;
            }
        }

        if (newHitBlock != lastOutlineBlock)
        {
            if (lastOutlineBlock != null)
            {
                setOutline(lastOutlineBlock, false);
            }

            if (newHitBlock != null)
            {
                setOutline(newHitBlock, true);
            }

            lastOutlineBlock = newHitBlock;
        } else if(lastOutlineBlock == null && newHitBlock != null)
        {
            setOutline(newHitBlock, true);
            lastOutlineBlock = newHitBlock;
        }
    }
    private void PerformBreakRaycast()
    {
        RaycastHit[] hits = new RaycastHit[(int)ContactRange];
        Ray ray = new Ray(cameraObject.transform.position, cameraObject.transform.forward);
        hits = Physics.RaycastAll(ray, ContactRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        HandleBreakBlock(hits);
    }
    private void UpdateBlockState(GameObject target, bool state)
    {
        //GridData targetData = target.GetComponent<GridData>();
        //if (targetData == null)
        //{
        //    Debug.LogError("Target object does not have a GridData component.");
        //    return;
        //}

        //foreach (Transform gridObjs in gridContainer.transform)
        //{
        //    GridData gridData = gridObjs.GetComponent<GridData>();

        //    if (gridData == null)
        //    {
        //        Debug.LogError("Grid object does not have a GridData component.");
        //        continue;
        //    }

        //    int targetCellX = targetData.cellX;
        //    int targetCellY = targetData.cellY;
        //    int targetHeight = targetData.cellHeight;

        //    int gridCellX = gridData.cellX;
        //    int gridCellY = gridData.cellY;
        //    int gridHeight = gridData.cellHeight;

        //    int cellXDifference = Mathf.Abs(targetCellX - gridCellX);
        //    int cellYDifference = Mathf.Abs(targetCellY - gridCellY);
        //    int heightDifference = Mathf.Abs(targetHeight - gridHeight);

        //    if ((cellXDifference == 1 && cellYDifference == 0 && heightDifference == 0) ||
        //    (cellXDifference == 0 && cellYDifference == 1 && heightDifference == 0) ||
        //    (cellXDifference == 0 && cellYDifference == 0 && heightDifference == 1))
        //    {
        //        if (gridHeight != 0)
        //        {
        //            gridObjs.gameObject.SetActive(state);
        //        }
        //    }
        //}
    }
    private void HandleOutlineBlock(RaycastHit[] hits)
    {
        bool stopLoop = false;
        if (hits.Length > 0)
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            for (int i = 0; i < hits.Length && stopLoop == false; i++)
            {

                if (hits[i].collider.isTrigger && hits[i].collider.CompareTag("Grid"))
                {
                    if (hits[i].collider.transform.childCount == 0)
                    {
                        if (lastOutlineBlock != null)
                        {
                            setOutline(lastOutlineBlock, false);
                        }
                        continue;
                    }
                    else
                    {
                        if (hits[i].collider.transform.GetChild(0) != null)
                        {
                            stopLoop = true;
                            setOutline(hits[i].collider.gameObject, true);
                            break;
                        }
                    }
                }

            }
        }
    }
    private void setOutline(GameObject target, bool state)
    {
        GridData gridData = target.transform.parent.GetComponent<GridData>();
        Outline outline = target.GetComponent<Outline>();
        if (outline == null)
        {
            outline = target.AddComponent<Outline>();

        }
        if (state)
        {
            if(lastOutlineBlock != null)
            {
                Outline outline1 = lastOutlineBlock.GetComponent<Outline>();
                outline1.OutlineWidth = 0f;
            }
            lastOutlineBlock = target;
            outline.OutlineWidth = 10f;
            if(gridData.isUtility == true)
            {
                switch(gridData.originInteractType)
                {
                    case InteractType.Body:
                        if(gridData.originGameObjectId == -1 && gridData.id >= 0)
                        {
                            outline.OutlineColor = Color.cyan;
                        } else
                        outline.OutlineColor = Color.green;
                        break;
                    case InteractType.Head:
                        if (gridData.id >= 0)
                            outline.OutlineColor = Color.red;
                        else
                            outline.OutlineColor = Color.magenta;
                        break;
                }
            } 
        } else
        {
            outline.OutlineWidth = 0f;
        }
    }
    private void HandlePlaceBlock(RaycastHit[] hits, Ray ray)
    {
        //Ray calculation
        Vector3 rayDirection = -ray.direction;
        rayDirection.y = 0f;
        float angle = Mathf.Atan2(rayDirection.z, rayDirection.x) * Mathf.Rad2Deg;
        float roundedAngle = Mathf.Round(angle / 90f) * 90f;
        float roundedAngleRad = roundedAngle * Mathf.Deg2Rad;
        Vector3 rotatedDirection = new Vector3(Mathf.Cos(roundedAngleRad), 0f, Mathf.Sin(roundedAngleRad));
        currentHitObject = null;
        bool stopLoop = false;
        if (hitList != null && hitList.Count > 0) 
        {
            hitList.Clear();
        }
        if (hits.Length > 0)
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            for (int i = 0; i < hits.Length && stopLoop == false; i++)
            {
                if (hits[i].collider.gameObject != null)
                {
                    if (hits[i].collider.isTrigger && hits[i].collider.CompareTag("Grid"))
                    {
                        if (hits[i].collider.transform.childCount == 0)
                        {
                            hitList.Add(hits[i].collider.gameObject);
                            continue;
                        }
                        else
                        {
                            if (hits[i].collider.transform.GetChild(0) != null && hitList.Count > 0)
                            {
                                stopLoop = true;
                                currentHitObject = hits[i].collider.gameObject;
                                HandleBlockPlace(hitList[hitList.Count - 1], Quaternion.LookRotation(rotatedDirection));
                                return;

                            }
                        }
                    }
                }
            }
            if (hitList.Count > 0 && stopLoop == false)
            {
                GridData gridData = hitList[hitList.Count - 1].GetComponent<GridData>();
                if (gridData.cellHeight == 0)
                {

                    if (placeBlockObject != null)
                    {
                        currentHitObject = hitList[hitList.Count - 1];
                        HandleBlockPlace(hitList[hitList.Count - 1], Quaternion.LookRotation(rotatedDirection));
                    }
                }
            }
        }

    }
    private void HandleBreakBlock(RaycastHit[] hits)
    {
        bool stopLoop = false;
        if (hits.Length > 0)
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            for (int i = 0; i < hits.Length && stopLoop == false; i++)
            {


                if (hits[i].collider.isTrigger && hits[i].collider.CompareTag("Grid"))
                {
                    if (hits[i].collider.transform.childCount == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (hits[i].collider.transform.GetChild(0) != null)
                        {
                            stopLoop = true;
                            HandleBlockBreak(hits[i].collider.gameObject);
                            UpdateBlockState(hits[i].collider.gameObject, false);
                            
                            break;
                        }
                    }
                }

            }
        }
    }
    int count = 0;
    public bool isTokenAffordable(int cost, GridData gridData = default(GridData), string name = null)
    {
            if (tokenManager != null)
            {
                if (tokenManager.getTokens() - cost >= 0)
                {
                    tokenManager.addTokens(-cost);
                if (gridData != null)
                {
                    gridData.tokenCost = cost;
                }
                    UpdateTokenDisplay(tokenManager.getTokens());
                    return true;
                }
                else
                {
                    InventoryBehaviour inventoryBehaviour = player.GetComponent<InventoryBehaviour>();
                    if (inventoryBehaviour != null && tokenManager != null)
                    {
                    if (name == null)
                    {
                        inventoryBehaviour.StartFadeInText("Unable to afford " + blockData.blockData[currentBlockId].blockModel.name + ". Need " + -(tokenManager.getTokens() - cost) + " more tokens", Color.red);
                    } else
                    {
                        inventoryBehaviour.StartFadeInText("Unable to afford " + name + ". Need " + -(tokenManager.getTokens() - cost) + " more tokens", Color.red);
                    }
                    } 
                    return false;
                }
            }
        
        return false;
    }
    public void SetFadeinText(string text, Color color = default)
    {
        if (color == default)
        {
            color = Color.red;
        }
        InventoryBehaviour inventoryBehaviour = player.GetComponent<InventoryBehaviour>();
        if (inventoryBehaviour != null)
        {

                inventoryBehaviour.StartFadeInText(text, color);
            
        }
    }
    public static float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }
    private void HandleBlockPlace(GameObject hitObject, Quaternion rotation = default)
    {
        if(rotation == default)
        {
            rotation = Quaternion.identity;
        }
        bool isAffordable = true;
        GridData gridData = hitObject.GetComponent<GridData>();
        GridData hitObjectData = currentHitObject.GetComponent<GridData>();
        
        if(gridData != null)
        {
            Vector3 eulerRotation = rotation.eulerAngles;
            eulerRotation.x = NormalizeAngle(eulerRotation.x);
            eulerRotation.y = NormalizeAngle(eulerRotation.y);
            eulerRotation.z = NormalizeAngle(eulerRotation.z);
            if (tokenManager != null)
            {
                isAffordable = isTokenAffordable(TokenManager.GetTokenCost(currentBlockId), gridData);
            }
            if(blockData.blockData[currentBlockId].isUtility != true)
            gridData.blockId = CurrentBlockId;
            gridData.Rotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
            gridData.isDefense = blockData.blockData[currentBlockId].isDefense;
        }
        // Handle the trigger collider hit object
        // Example: Call a function on the collided object
        if (isAffordable)
        {
            if (blockData.blockData[currentBlockId].isUtility == false)
            {
                if (PlaceBlockObject == null)
                {
                    //If block not a Utility & No model, create a cube object instead
                    if (blockData.blockData[currentBlockId].isUtility == false)
                    {
                        CreateCube(hitObject);
                    }
                    //Utility Condition and function
                }
                else
                {
                    CreateBlock(hitObject, rotation);
                    if (currentHitObject != null)
                    {
                        GridData gridDataTarget = currentHitObject.GetComponent<GridData>();

                        if (gridDataTarget.isUtility == true)
                        {
                            InteractBlock(hitObject, gridDataTarget.originInteractType);
                        }
                    }
                }
                count++;
            }
            if (blockData.blockData[currentBlockId].isUtility == true)
            {
                UtilityIssueDetection(hitObjectData);
                if (hitObjectData.originInteractType != InteractType.Body && blockData.blockData[currentBlockId].utilityType == InteractType.Body)
                {
                    InteractBlock(currentHitObject, InteractType.Body, true);
                }
                if (hitObjectData.originInteractType == InteractType.Body && hitObjectData.id == -1 && blockData.blockData[currentBlockId].utilityType == InteractType.Head)
                {
                    
                    InteractBlock(currentHitObject, InteractType.Head, true);
                }
            }
            UpdateBlockState(hitObject, true);
        }
        
        Debug.Log("Placed blocks: " + count);
    }
    private void UtilityIssueDetection(GridData gridData)
    {
        InventoryBehaviour inventoryBehaviour = player.GetComponent<InventoryBehaviour>();
        if(gridData.originInteractType == InteractType.none && blockData.blockData[currentBlockId].utilityType == InteractType.Head)
        {
            inventoryBehaviour.StartFadeInText("Unable to set block to Head. Reason: Not a secondary body block.", Color.red, 2f);
        }
        if (gridData.originInteractType == InteractType.Body && gridData.id != -1 && blockData.blockData[currentBlockId].utilityType == InteractType.Head)
        {
            inventoryBehaviour.StartFadeInText("Unable to set block to Head. Reason: The Block has a main body type.", Color.red, 2f);
        }
        if (gridData.originInteractType == InteractType.Head && blockData.blockData[currentBlockId].utilityType == InteractType.Head)
        {
            inventoryBehaviour.StartFadeInText("Unable to set block to Head. Reason: Already a Head type Block.", Color.red, 2f);
        }
        if (gridData.originInteractType == InteractType.Body && blockData.blockData[currentBlockId].utilityType == InteractType.Body)
        {
            inventoryBehaviour.StartFadeInText("Unable to set block to Body. Reason: Already a Body type Block.", Color.red, 2f);
        }
    }
    private void HandleBlockBreak(GameObject hitObject)
    {
        Debug.Log("Trigger collider detected: " + hitObject.name);
        // Handle the trigger collider hit object
        // Example: Call a function on the collided object
        GridData gridData = hitObject.GetComponent<GridData>();
        if(gridData != null)
        {
            TokenManager tokenManager = player.GetComponent<TokenManager>();
            if(tokenManager != null)
            {
                tokenManager.addTokens(gridData.tokenCost);
                UpdateTokenDisplay(tokenManager.getTokens());
            }
            gridData.reset();
        }
        count--;
        Debug.Log("Placed blocks: " + count);
        Destroy(hitObject.transform.GetChild(0).gameObject);
    }
    public void UpdateToken(int amount)
    {
        TokenManager tokenManager = player.GetComponent<TokenManager>();
        if (tokenManager != null)
        {
            tokenManager.addTokens(amount);
            UpdateTokenDisplay(tokenManager.getTokens());
        }
    }
    void CreateCube(GameObject parentObj)
    {
        if (parentObj.transform.childCount == 0)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.localPosition = parentObj.transform.localPosition;
            cube.transform.SetParent(parentObj.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one;
        }
    }
    void CreateBlock(GameObject parentObj, Quaternion rotation)
    {
        if (parentObj.transform.childCount == 0)
        {
            GameObject block = Instantiate(PlaceBlockObject, parentObj.transform.position, Quaternion.identity);
            block.transform.localPosition = parentObj.transform.localPosition;
            block.transform.SetParent(parentObj.transform);
            block.transform.rotation = rotation;
            block.transform.localPosition = Vector3.zero;
            block.transform.localScale = Vector3.one;
        }
    }
    //private void HandleRegularHit(GameObject hitObject)
    //{
    //    Debug.Log("Regular collider detected: " + hitObject.name);
    //    // Handle the regular collider hit object
    //    // Example: Apply damage to the collided object
    //    hitObject.GetComponent<Health>().TakeDamage(10);
    //}
    private void InteractBlock(GameObject target, InteractType interactType, bool isOrigin = false)
    {
        GridData gridData = target.GetComponent<GridData>();
        GridData originGridData = currentHitObject.GetComponent<GridData>();
        switch(interactType)
        {
            case InteractType.Body:
            case InteractType.Head:
                if (isOrigin == true)
                {
                    gridData.id = blockData.GetId();
                    gridData.originInteractType = interactType;
                    gridData.isUtility = true;
                    if (interactType == InteractType.Body)
                    {
                        gridData.originGameObjectId = -1;
                    }
                } else 
                {
                    if (originGridData.id > -1)
                    {
                        gridData.originGameObjectId = originGridData.id;
                    } else
                    {
                        gridData.originGameObjectId = originGridData.originGameObjectId;
                    }
                    Debug.Log(gridData.originGameObjectId + " - " + originGridData.id);
                    gridData.originInteractType = originGridData.originInteractType;
                    gridData.isUtility = true;
                }
                break;


        }
        setOutline(target.transform.GetChild(0).gameObject, true);
    }
    public void ResetGrid()
    {
        defenseCount = 0;
        foreach (Transform child in gridContainer.transform)
        {
            Destroy(child.gameObject);
        }
    }
    private void ToggleCellState(int row, int col)
    {
        grid[row, col].ToggleState();

        // Adjust the visual representation of the cell based on its state
        Renderer cellRenderer = cellVisuals[row, col].GetComponent<Renderer>();
        cellRenderer.material.color = grid[row, col].IsAlive ? Color.red : Color.white;
    }
    public void SaveStructure()
    {
        if (path == "Default")
        {
            return;
        }
        StructureSerializer.SaveObject(gridContainer, path);

       
    }
    public void LoadStructure()
    {
        int cost = 0;
        StructureStorage[] structureStorages = StructureSerializer.LoadObject(path);
        if (structureStorages != null)
        {
            foreach (StructureStorage structureStorage in structureStorages)
            {
                //Debug.Log("Pos: " + structureStorage.cellPos[0] + ", " + structureStorage.cellPos[1] + ", " + structureStorage.cellPos[2] + "\nStructure: " + structureStorage.structureId);
                cost += structureStorage.tokenCost;
            }
            Debug.Log("cost: " + cost);
            string structureName = StructureSerializer.GetFileName(path);
            GameObject gameObject = default;
            if (isTokenAffordable(cost, default, structureName))
            {
                gameObject = GenerateStructure(structureStorages);
                gameObject.name = structureName;
            }

           
        }
    }
    public void AddDefense(int value)
    {
        defenseCount += value;
    }
    public bool IsMaxDefenseReached(int blockId)
    {
        if(blockData.blockData[blockId].isDefense)
        {
            if (defenseCount < m_MaxDefenseCount)
            {
                defenseCount++;
                return false;
            }
            else
            {
                SetFadeinText("Max Defense Reached! (" + m_MaxDefenseCount + ")");
                return true;
            }
        }
        return false;
    }
    public void LoadStructure(string filePath = default)
    {
        if(filePath == default || filePath == "")
        {
            filePath = path;
            if(filePath == "Default")
            {
                return;
            }
        }
        int cost = 0;
        
        StructureStorage[] structureStorages = StructureSerializer.LoadObject(filePath);
        foreach (StructureStorage structureStorage in structureStorages)
        {
            //Debug.Log("Pos: " + structureStorage.cellPos[0] + ", " + structureStorage.cellPos[1] + ", " + structureStorage.cellPos[2] + "\nStructure: " + structureStorage.structureId);
            cost += structureStorage.tokenCost;

        }
        Debug.Log("cost: " + cost);
        GameObject gameObject = default;
        string structureName = StructureSerializer.GetFileName(path);
        if (isTokenAffordable(cost, default, structureName))
        {
            gameObject = GenerateStructure(structureStorages);
            gameObject.name = structureName;
        }
      
    }
   
    public GameObject GenerateStructure(StructureStorage[] structureStorage, Vector3 position = default(Vector3))
    {
        if (position == default(Vector3))
        {
            position = Vector3.zero; // Set a default value here
        }
        int count = 0;
        GameObject structure = new GameObject();
        structure.transform.localPosition = position;
        int[] utilityIdList = new int[0];

        List<GameObject> utilityList = new List<GameObject>();
        for (int i=0; i< structureStorage.Length; i++)
        {
            
            GameObject block = Instantiate(blockData.blockData[structureStorage[i].structureId].blockModel, Vector3.zero, Quaternion.identity);
            block.name = blockData.blockData[structureStorage[i].structureId].blockModel.name + ": " + i;
            if (structureStorage[i].isUtility == false)
            {
                block.transform.SetParent(structure.transform);
            } else
            {
                utilityList.Add(block);
            }
            count++;
            block.transform.position = new Vector3((structureStorage[i].cellPos[0] - numRows/2) * cellSize, (structureStorage[i].cellPos[1] + cellSize) * cellSize, (structureStorage[i].cellPos[2] - numColumns/2) * cellSize);
            block.transform.eulerAngles = new Vector3(structureStorage[i].Rotation[0], structureStorage[i].Rotation[1], structureStorage[i].Rotation[2]);
            block.transform.localScale = new Vector3(structureStorage[i].Scale[0] * cellSize, structureStorage[i].Scale[1] * cellSize, structureStorage[i].Scale[2] * cellSize);

            GridData gridData = block.AddComponent<GridData>();
            gridData.SetData(structureStorage[i]);


        }

        CombineGameObjects(structure);
        foreach(GameObject gameObject in utilityList)
        {
            gameObject.transform.SetParent(structure.transform);
        }


        Debug.Log("Count: " + count);
        return structure;
    }
    private void CombineGameObjects(GameObject parentObject)
    {
        MeshCombiner meshCombiner = parentObject.AddComponent<MeshCombiner>();
        meshCombiner.CreateMultiMaterialMesh = true;
        meshCombiner.DestroyCombinedChildren = true;
        meshCombiner.CombineMeshes(true);
        
        Destroy(meshCombiner);
        MeshCollider meshCollider = parentObject.AddComponent<MeshCollider>();
        MeshFilter meshFilter = parentObject.GetComponent<MeshFilter>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        meshCollider.convex = false;
        meshCollider.providesContacts = true;
    }
    public void GenerateStructureWithGrid(string filePath = default)
    {
        
        if(filePath == default || filePath == "default")
        {
            filePath = path;
            Debug.Log("Path: " + filePath);
        }
        if(isEditable == false && isMenuOpen == false)
        {
            return;
        }
        tokenManager.setTokens(tokenManager.initialTokens);
        int cost = 0;
        ResetGrid();
        StructureStorage[] structureStorages = StructureSerializer.LoadObject(filePath);

            foreach (StructureStorage structureStorage in structureStorages)
            {

                    GameObject block = Instantiate(blockData.blockData[structureStorage.structureId].blockModel, Vector3.zero, Quaternion.identity, gridContainer.transform);
                    block.transform.position = new Vector3(structureStorage.cellPos[0], structureStorage.cellPos[1], structureStorage.cellPos[2]);
                    block.transform.eulerAngles = new Vector3(structureStorage.Rotation[0], structureStorage.Rotation[1], structureStorage.Rotation[2]);
                    block.transform.localScale = new Vector3(structureStorage.Scale[0], structureStorage.Scale[1], structureStorage.Scale[2]);
                    GridData gridData = block.AddComponent<GridData>();
                    gridData.isAutoRotatable = structureStorage.isAutoRotatable;
                    gridData.cellX = (int)structureStorage.cellPos[0];
                    gridData.cellY = (int)structureStorage.cellPos[2];
                    gridData.cellHeight = (int)structureStorage.cellPos[1];
                    gridData.blockId = structureStorage.structureId;
                    gridData.Rotation = new Vector3(structureStorage.Rotation[0], structureStorage.Rotation[1], structureStorage.Rotation[2]);
                    gridData.Scale = new Vector3(structureStorage.Scale[0], structureStorage.Scale[1], structureStorage.Scale[2]);
                    gridData.tokenCost = structureStorage.tokenCost;
                    gridData.isUtility = structureStorage.isUtility;
                    gridData.isDefense = structureStorage.isDefense;
            if(gridData.isDefense)
            {
                defenseCount--;
            }
                    gridData.id = structureStorage.id;
                    block.tag = "Grid";
                    block.layer = LayerMask.NameToLayer("Grid");
                    gridData.originGameObjectId = structureStorage.originGameObjectId;
                    gridData.originInteractType = structureStorage.originInteractType;
                    cost += structureStorage.tokenCost;
            BoxCollider boxCollider = block.GetComponent<BoxCollider>();
            if(boxCollider!= null)
            {
                boxCollider.size += new Vector3(0.0001f, 0.0001f, 0.0001f);
            }
            MeshRenderer meshRenderer = block.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            }

        
        isTokenAffordable(cost, default, StructureSerializer.GetFileName(filePath));
    }
    public void UpdateTokenDisplay(int newAmount)
    {
        if(tokenAmountTextDisplay != null)
        {
            TMP_Text textField = tokenAmountTextDisplay.GetComponent<TMP_Text>();
            if(textField != null)
            {
                textField.text = newAmount.ToString();
            }
        }
        
    }
    public void SetNameFromPath()
    {
        name = StructureSerializer.SetStructureNameFromFile(path, true);
    }
    public void SaveStructureImg()
    {
        if(path == "Default")
        {
            return;
        }
        StructureStorage[] structureStorages = StructureSerializer.LoadObject(path);
        ModelPictureSaver.CaptureAndSaveImage(captureCamera ,GenerateStructure(structureStorages), "/StructureData/StructureImg/", name);
    }
    public void SaveTempStructureImg()
    {
        int oldMask = captureCamera.cullingMask;
        captureCamera.cullingMask = -1;
        ModelPictureSaver.CaptureAndSaveImage(captureCamera, gridContainer, "/StructureData/Temp/", "tempStructureImg", false);
        captureCamera.cullingMask = oldMask;
    }

    #region File Upload / Export
    public void UploadStructure()
    {
        var extensions = new[] {
        new ExtensionFilter("Json Files", "json")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Upload File", "", extensions, false);
        if(paths.Length == 0 || paths[0] == null)
        {
            return;
        }
        StructureSerializer.UploadStructure(paths[0]);
    }
    public void ExportStructure()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Export File", "", false);
        if (paths.Length == 0 || paths[0] == null)
        {
            return;
        }
        StructureSerializer.ExportStructure(paths[0],Application.persistentDataPath + path);
    }
    #endregion
}

public class GridCell
{
    public int Row { get; private set; }
    public int Column { get; private set; }
    public bool IsAlive { get; private set; }

    public GridCell(int row, int column)
    {
        Row = row;
        Column = column;
        IsAlive = false;
    }

    public void ToggleState()
    {
        IsAlive = !IsAlive;
    }
}
