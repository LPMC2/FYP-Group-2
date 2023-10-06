using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    [Header("User Settings")]
    [SerializeField] private GameObject player;
    [Header("Display Settings")]
    [SerializeField] private GameObject displayActionBar;
    [Header("Grid Settings")]
    public int numRows = 5;
    public int numColumns = 5;
    public int numHeight = 5;
    public float cellSize = 1f;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private GameObject gridContainer;
    [SerializeField] private GameObject gridBlockObj;
    [SerializeField] private GameObject gridVisualLayer;
    [SerializeField] private GameObject gridVisibleObj;
    [SerializeField] private float ContactRange = 10f;
    [SerializeField] private GameObject placeBlockObject;
    [SerializeField] private int currentBlockId;
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
    [SerializeField] private string path = "/SavedObjectData.json";
    [Header("Place/Break Settings")]
    [SerializeField] private bool isEditable = true;
    private bool canPlace = true; // Flag to track if placing is allowed
    private bool canBreak = true; // Flag to track if breaking is allowed
    [SerializeField] private float placeCooldown = 0.05f; // Cooldown duration for placing (in seconds)
    [SerializeField] private float breakCooldown = 0.05f; // Cooldown duration for breaking (in seconds)
    private float placeTimer = 0.0f; // Timer for placing cooldown
    private float breakTimer = 0.0f; // Timer for breaking cooldown
    private GridCell[,] grid;
    private GameObject[,] cellVisuals;
    private GameObject lastOutlineBlock;
    private List<GameObject> hitList = new List<GameObject>();
    private BlockSO blockData;
    private void Awake()
    {
        blockData = BlockManager.BlockData;
    }
    private void Start()
    {
        if (isEditable)
        {
            CreateGrid();
            CreateVisualGrid();
            CreateInteractableGrid();
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
    private void CreateInteractableGrid()
    {
        cellVisuals = new GameObject[numRows, numColumns];

        // Calculate the offset to position the grid at the center of the current holder object
        Vector3 gridOffset = new Vector3((numRows - 1) * cellSize * 0.5f, -(cellSize/2), (numColumns - 1) * cellSize * 0.5f);

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
                    if(height > -1 && cellInteractable != null)
                    {
                        cellInteractable.SetActive(false);
                    }
                }
            }
        }
    }

    private void Update()
    {
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
            PerformPlaceRaycast();
            canPlace = false;
        }

        if (Input.GetMouseButtonDown(0) && canBreak)
        {
            PerformBreakRaycast();
            canBreak = false;
        }
        UpdateLoadGrid();
    }
    private void UpdateLoadGrid()
    {
        foreach (Transform target in gridContainer.transform)
        {
            float distance = Vector3.Distance(cameraObject.transform.position, target.position);
            if(distance <= ContactRange && !target.gameObject.activeInHierarchy)
            {
                target.gameObject.SetActive(true);
            } else if(distance > ContactRange && target.childCount == 0 && target.gameObject.activeInHierarchy)
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
        HandlePlaceBlock(hits);
        Debug.DrawRay(cameraObject.transform.position, cameraObject.transform.forward * ContactRange, Color.red, 5f);
    }
    private void PerformOutlineRaycast()
    {
        RaycastHit[] hits = new RaycastHit[(int)ContactRange];
        Ray ray = new Ray(cameraObject.transform.position, cameraObject.transform.forward);
        hits = Physics.RaycastAll(ray, ContactRange, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide);
        Physics.RaycastNonAlloc(ray, hits);
        HandleOutlineBlock(hits);
       
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
        } else
        {
            outline.OutlineWidth = 0f;
        }
    }
    private void HandlePlaceBlock(RaycastHit[] hits)
    {
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
                                HandleBlockPlace(hitList[hitList.Count - 1]);
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
                    HandleBlockPlace(hitList[hitList.Count - 1]);
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
    public bool isTokenAffordable(TokenManager tokenManager, GridData gridData)
    {
        if (gridData != null)
        {
            if (tokenManager != null)
            {
                int cost = tokenManager.GetTokenCost(currentBlockId);
                if (tokenManager.getTokens() - cost >= 0)
                {
                    tokenManager.addTokens(-cost);
                    gridData.tokenCost = cost;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        return false;
    }
    private void HandleBlockPlace(GameObject hitObject)
    {
        TokenManager tokenManager = player.GetComponent<TokenManager>();
        bool isAffordable = true;
        GridData gridData = hitObject.GetComponent<GridData>();
        if(gridData != null)
        {
            
            if(tokenManager != null)
            {
                isAffordable = isTokenAffordable(tokenManager, gridData);
            }
            gridData.blockId = CurrentBlockId;
        }
        // Handle the trigger collider hit object
        // Example: Call a function on the collided object
        if (isAffordable)
        {
            if (PlaceBlockObject == null)
            {
                CreateCube(hitObject);
            }
            else
            {
                CreateBlock(hitObject);
            }
            UpdateBlockState(hitObject, true);
        } else
        {
            InventoryBehaviour inventoryBehaviour = player.GetComponent<InventoryBehaviour>();
            if(inventoryBehaviour!= null && tokenManager != null)
            {
                inventoryBehaviour.StartFadeInText("Unable to afford " + blockData.blockData[currentBlockId].blockModel.name + ". Need " + -(tokenManager.getTokens() - tokenManager.GetTokenCost(currentBlockId)) + " more tokens", Color.red);
            }
        }
        count++;
        Debug.Log("Placed blocks: " + count);
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
            }
            gridData.reset();
        }
        count--;
        Debug.Log("Placed blocks: " + count);
        Destroy(hitObject.transform.GetChild(0).gameObject);
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
    void CreateBlock(GameObject parentObj)
    {
        if (parentObj.transform.childCount == 0)
        {
            GameObject block = Instantiate(PlaceBlockObject, parentObj.transform.position, Quaternion.identity);
            block.transform.localPosition = parentObj.transform.localPosition;
            block.transform.SetParent(parentObj.transform);
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
    private void ToggleCellState(int row, int col)
    {
        grid[row, col].ToggleState();

        // Adjust the visual representation of the cell based on its state
        Renderer cellRenderer = cellVisuals[row, col].GetComponent<Renderer>();
        cellRenderer.material.color = grid[row, col].IsAlive ? Color.red : Color.white;
    }
    public void SaveStructure()
    {
        if (isEditable)
        {
            StructureSerializer.SaveObject(gridContainer, path);
        }
    }
    public void LoadStructure()
    {
        StructureStorage[] structureStorages = StructureSerializer.LoadObject(path);
        if (structureStorages != null)
        {
            foreach (StructureStorage structureStorage in structureStorages)
            {
                Debug.Log("Pos: " + structureStorage.cellPos[0] + ", " + structureStorage.cellPos[1] + ", " + structureStorage.cellPos[2] + "\nStructure: " + structureStorage.structureId);
            }
            GameObject gameObject = GenerateStructure(structureStorages);
        }
    }
    public void LoadStructure(string filePath)
    {
        StructureStorage[] structureStorages = StructureSerializer.LoadObject(filePath);
        foreach (StructureStorage structureStorage in structureStorages)
        {
            Debug.Log("Pos: " + structureStorage.cellPos[0] + ", " + structureStorage.cellPos[1] + ", " + structureStorage.cellPos[2] + "\nStructure: " + structureStorage.structureId);
        }
        GameObject gameObject = GenerateStructure(structureStorages);
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
        for(int i=0; i< structureStorage.Length; i++)
        {
            GameObject block = Instantiate(blockData.blockData[structureStorage[i].structureId].blockModel, Vector3.zero, Quaternion.identity);
            block.transform.SetParent(structure.transform);
            count++;
            block.transform.position = new Vector3((structureStorage[i].cellPos[0] - numRows/2) * cellSize, (structureStorage[i].cellPos[1] + cellSize) * cellSize, (structureStorage[i].cellPos[2] - numColumns/2) * cellSize);
            block.transform.eulerAngles = new Vector3(structureStorage[i].Rotation[0], structureStorage[i].Rotation[1], structureStorage[i].Rotation[2]);
            block.transform.localScale = new Vector3(structureStorage[i].Scale[0] * cellSize, structureStorage[i].Scale[1] * cellSize, structureStorage[i].Scale[2] * cellSize);

            GridData gridData = block.AddComponent<GridData>();
            gridData.isAutoRotatable = structureStorage[i].isAutoRotatable;
            gridData.cellX = (int)structureStorage[i].cellPos[0];
            gridData.cellY = (int)structureStorage[i].cellPos[2];
            gridData.cellHeight = (int)structureStorage[i].cellPos[1];
            gridData.blockId = structureStorage[i].structureId;
            gridData.Rotation = new Vector3(structureStorage[i].Rotation[0], structureStorage[i].Rotation[1], structureStorage[i].Rotation[2]);
            gridData.Scale = new Vector3(structureStorage[i].Scale[0], structureStorage[i].Scale[1], structureStorage[i].Scale[2]);
        }
        Debug.Log("Count: " + count);
        return structure;
    }
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
