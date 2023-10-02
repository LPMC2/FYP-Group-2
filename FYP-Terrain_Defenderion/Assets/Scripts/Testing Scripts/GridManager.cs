using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
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
    [Header("Place/Break Settings")]
    private bool canPlace = true; // Flag to track if placing is allowed
    private bool canBreak = true; // Flag to track if breaking is allowed
    [SerializeField] private float placeCooldown = 0.05f; // Cooldown duration for placing (in seconds)
    [SerializeField] private float breakCooldown = 0.05f; // Cooldown duration for breaking (in seconds)
    private float placeTimer = 0.0f; // Timer for placing cooldown
    private float breakTimer = 0.0f; // Timer for breaking cooldown
    private GridCell[,] grid;
    private GameObject[,] cellVisuals;
    private GameObject lastOutlineBlock;
   [SerializeField] private List<GameObject> hitList = new List<GameObject>();
    private void Start()
    {
        CreateGrid();
        CreateVisualGrid();
        CreateInteractableGrid();
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
                    if(height != 0)
                    {
                        //cellInteractable.SetActive(false);
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
        hitList.Clear();
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
        hitList.Clear();
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
        hitList.Clear();
        if (hits.Length > 0)
        {
            Array.Sort(hits, (RaycastHit x, RaycastHit y) => x.distance.CompareTo(y.distance));
            for (int i = 0; i < hits.Length && stopLoop == false; i++)
            {

                Debug.Log(hits[i].collider.name);
                if (hits[i].collider.isTrigger && hits[i].collider.CompareTag("Grid"))
                {
                    if (hits[i].collider.transform.childCount == 0)
                    {
                        hitList.Add(hits[i].collider.gameObject);
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
    private void HandleBlockPlace(GameObject hitObject)
    {
        Debug.Log("Trigger collider detected: " + hitObject.name);
        // Handle the trigger collider hit object
        // Example: Call a function on the collided object
        CreateCube(hitObject);
        UpdateBlockState(hitObject, true);
    }
    private void HandleBlockBreak(GameObject hitObject)
    {
        Debug.Log("Trigger collider detected: " + hitObject.name);
        // Handle the trigger collider hit object
        // Example: Call a function on the collided object
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
