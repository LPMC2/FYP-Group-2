using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] TemplateType templateType;
    [SerializeField] private GridType gridType;
    [SerializeField] private GameObject userObject;
    [SerializeField]
    private DisplayBehaviour displayBehaviour;
    [SerializeField]
    private Camera playerCamera;
    private TokenManager tokenManager;
    [SerializeField] private Vector3 buildOffset = Vector3.zero;
    [SerializeField]
    private float minBuildDistance = 1f;
    [SerializeField]
    private float maxBuildDistance = 10f;
    private bool buildModeOn = false;
    public bool BuildMode { get { return buildModeOn; } set { buildModeOn = value; } }
    private bool canBuild = false;
    [SerializeField]
    private List<GridCell> gridCells = new List<GridCell>();
    #region gridcell list manager
    public bool ValueEquals(int row, int column, int height)
    {
        foreach(GridCell gridCell in gridCells)
        {
            if(gridCell.Row == row && gridCell.Column == column && gridCell.Height == height)
            {
                return true;
            }
        }
        return false;

    }
    public void AddValue(int row, int column, int height)
    {
        gridCells.Add(new GridCell(row, column, height));
    }
    public void RemoveValue(int row, int column, int height)
    {
        Debug.Log("Remove Value: " + row + " " + column + " " + height);
        gridCells.RemoveAll(item => item.Row == row && item.Column == column && item.Height == height);
    }
    public void RemoveAll()
    {
        gridCells.Clear();
    }
    #endregion
    private BlockSO bSys;

    [SerializeField]
    private LayerMask buildableSurfacesLayer;

    private Vector3 buildPos;

    private GameObject currentTemplateBlock;

    [SerializeField]
    private GameObject templateGameobject;
    public GameObject TemplateGameObject { get { return templateGameobject; } }
    [SerializeField]
    private GameObject blockPrefab;
    [Header("Material Settings")]
    [SerializeField]
    private Material templateMaterial;
    [SerializeField]
    private Material deleteMaterial;
    [SerializeField]
    private Material highlightMaterial;
    [Header("Structure Settings")]
    [SerializeField]
    private StructureManager structureManager;
    private GridSize gridSize;
    private int blockSelectCounter = 0;
    private int currentStructure = -1;
    CollisionDetector colDetector;
    private void Start()
    {
        if (userObject == null)
            userObject = gameObject;
        buildModeOn = true;
        tokenManager = userObject.GetComponent<TokenManager>();
        if(playerCamera !=null)
        {
            shootingPoint = playerCamera.transform;
        }
    }
    private void OnDisable()
    {
        if (lastOutlineBlock != null)
        {
            setOutline(lastOutlineBlock, false);
            lastOutlineBlock = null;
        }
        if (templateGameobject != null)
        {
            Destroy(templateGameobject);
            templateGameobject = null;
        }
    }
    public void SetGridSize(int row, int col, int h, GridSize gridSize)
    {
        numRows = row;
        numColumns = col;
        numHeight = h;
        this.gridSize = gridSize;
    }
    public int numRows = 25;
    public int numColumns = 25;
    public int numHeight = 25;
    public float cellSize = 0.5f;
    private int blockID;
    private BlockSO blockSO;
    GridManager gridManager;
    public void SetGridGenerator(GridManager gridManager,BlockSO blockSO, int BlockID, int row, int col, int height, float size, Transform parent, Transform shootingPoint, float range)
    {
        this.gridManager = gridManager;
        this.blockSO = blockSO;
        blockID = BlockID;
        numRows = row;
        numColumns = col;
        numHeight = height;
        cellSize = size;
        this.parent = parent;
        this.shootingPoint = shootingPoint;
        playerCamera = shootingPoint.GetComponent<Camera>();
        this.range = range;
    }
    public void SetBlock(GameObject gameObject)
    {
        targetBlock = gameObject;
    }
    public GameObject GetCurrentStructure()
    {
        GameObject gameObject = structureManager.GetStructure(structureManager.CurrentStructure, false);
        if (gameObject == null)
        {
            reachMaxStructure = true;
        }
        else
            reachMaxStructure = false;
        return gameObject;
    }
    private GameObject GetTemplateGameObject()
    {
        GameObject templateGameobject = Instantiate(GetCurrentStructure(), buildPos, Quaternion.identity);
        templateGameobject.SetActive(true);
        templateGameobject.layer = LayerMask.NameToLayer("Default");
        //Assign Material
        MaterialBehaviour materialBehaviour = templateGameobject.AddComponent<MaterialBehaviour>();
        materialBehaviour.SetMaterial(templateMaterial);
        materialBehaviour.SetInitialMaterial(templateMaterial);
       
        //Assign Collision Detection
        CollisionDetector collisionDetector = templateGameobject.AddComponent<CollisionDetector>();
        collisionDetector.SetCollsiionType(CollisionType.Trigger);
        collisionDetector.OnCollision += SetCollisionHit;
        colDetector = collisionDetector;
        //Remove Rigidbody
        Rigidbody rigidbody = templateGameobject.GetComponent<Rigidbody>();
        Destroy(rigidbody);
        //Adjust the collider
        MeshCollider meshCollider = templateGameobject.GetComponent<MeshCollider>();
        meshCollider.enabled = false;
        BoxCollider boxCollider = templateGameobject.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.size = new Vector3(boxCollider.size.x - 0.01f, boxCollider.size.y - 0.01f, boxCollider.size.z - 0.01f);
        }
        return templateGameobject;
    }
    public void SetCollisionHit(bool state)
    {
        hitStructure = state;
        SetMaterial(hitStructure);
    }
    private void SetMaterial(bool state)
    {
        if (templateGameobject != null)
        {
            MaterialBehaviour materialBehaviour = templateGameobject.GetComponent<MaterialBehaviour>();
            if (state)
            {
                materialBehaviour.SetMaterial(deleteMaterial);
            }
            else
            {
                materialBehaviour.ResetMat();
            }
        }
    }
    private void Update()
    {
        if (buildModeOn)
        {
            PerformOutlineRaycast();
            RaycastHit buildPosHit;

            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, maxBuildDistance, buildableSurfacesLayer))
            {
                Vector3 point = buildPosHit.point;
                buildPos = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), Mathf.Round(point.z));

                if (IsValidPosition(buildPos))
                {
                    if (canBuild == false)
                    {
                        SetMaterial(true);
                        canBuild = true;
                    }
                    if (templateType == TemplateType.GameObject && templateGameobject == null && GetCurrentStructure() != null)
                    {
                        templateGameobject = GetTemplateGameObject();

                    }
                    if (templateGameobject != null)
                    {
                        templateGameobject.transform.position = buildPos + buildOffset;
                        Quaternion direction = GetDirection(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)));
                        Vector3 eulerRotation = direction.eulerAngles;
                        //newBlock.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
                        eulerRotation.x = GridManager.NormalizeAngle(eulerRotation.x);
                        eulerRotation.y = GridManager.NormalizeAngle(eulerRotation.y);
                        eulerRotation.z = GridManager.NormalizeAngle(eulerRotation.z);
                        templateGameobject.transform.eulerAngles = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);

                    }

                }
                else
                {
                    if(canBuild == true)
                    {
                        SetMaterial(false);
                        canBuild = false;
                    }
                    Destroy(templateGameobject);
                    templateGameobject = null;
                }
            }
            else
            {
                canBuild = false;
                Destroy(templateGameobject);
                templateGameobject = null;
            }
            if (Input.GetMouseButtonDown(0))
            {
                DestroyBlock(shootingPoint, range);
            }
            if(gridType == GridType.Structure && templateGameobject != null)
            {
                if(structureManager.CurrentStructure == -1)
                {
                    Destroy(templateGameobject);
                    templateGameobject = null;
                }
            }
            if(colDetector != null)
            {
                if(hitStructure != colDetector.isHit)
                {
                    hitStructure = colDetector.isHit;
                }
            }
        }

        if (!buildModeOn)
        {
        
            canBuild = false;
        }

        if (canBuild)
        {


            if (Input.GetMouseButtonDown(1))
            {
                PlaceBlock();
            }

        }
        if(!canBuild)
        {
            if (Input.GetMouseButtonDown(1))
            {
                ErrorMessage();
            }
        }
        if(gridType == GridType.Structure)
        {
            if(structureManager.CurrentStructure != currentStructure)
            {
                Destroy(templateGameobject);
                templateGameobject = null;
                currentStructure = structureManager.CurrentStructure;
            }
        }
    }
    private void PlaceBlock()
    {
        int cost = 0;
        if(hitStructure) { ErrorMessage(); return; }
        if (gridManager != null && gridType == GridType.Block)
        {
            cost = TokenManager.GetTokenCost(gridManager.CurrentBlockId);
            if (gridManager.PlaceBlockObject == null || gridManager.CurrentBlockId == -1) return;
            if (!gridManager.isTokenAffordable(cost, null))
            {
                return;
            }
            else if (gridManager.IsMaxDefenseReached(gridManager.CurrentBlockId))
            {
                return;
            }
        }
        if(gridType == GridType.Structure && GetCurrentStructure()!=null)
        {
            cost = GetCurrentStructure().GetComponent<GridData>().tokenCost;
           if (!tokenManager.isTokenAffordable(cost, structureManager.structurePoolings[structureManager.CurrentStructure].name) || templateGameobject == null || GetCurrentStructure() == null)
            {
                return;
            }
        }
        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);
        Vector3 originPos = Vector3.zero;
        GameObject newBlock = null;
        switch (gridType) 
        {
            case GridType.Block:
                newBlock = Instantiate(gridManager.PlaceBlockObject, buildPos, Quaternion.identity, parent);
                break;
            case GridType.Structure:
                newBlock = structureManager.GetStructure(structureManager.CurrentStructure);
                if (newBlock != null)
                {
                    originPos = newBlock.transform.position;
                    newBlock.transform.position = buildPos;
                }
                break;
        }
        if (newBlock == null) return;
        BoxCollider boxCollider = newBlock.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.size += new Vector3(0.0001f, 0.0001f, 0.0001f);
        }
        Quaternion direction = GetDirection(ray);
        GridData gridData = newBlock.GetComponent<GridData>();
        if(gridData == null)
        {
            gridData = newBlock.AddComponent<GridData>();
        }
        Vector3 eulerRotation = direction.eulerAngles;
        //newBlock.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
        eulerRotation.x = GridManager.NormalizeAngle(eulerRotation.x);
        eulerRotation.y = GridManager.NormalizeAngle(eulerRotation.y);
        eulerRotation.z = GridManager.NormalizeAngle(eulerRotation.z);
        Vector3 offsetPos = Vector3.zero;
        if (gridType == GridType.Block)
            offsetPos = GetOffsetPosFromDir(eulerRotation, gridManager.PlaceBlockObject.transform.position);
        if (gridType == GridType.Structure && GetCurrentStructure() != null)
            offsetPos = GetOffsetPosFromDir(eulerRotation, GetCurrentStructure().transform.position);
        if (newBlock.GetComponent<BoxCollider>() != null)
        {
            if(gridType == GridType.Block)
            newBlock.GetComponent<BoxCollider>().center += new Vector3(0f, 0f, gridManager.PlaceBlockObject.transform.position.x + gridManager.PlaceBlockObject.transform.position.z);
            if(gridType == GridType.Structure)
                newBlock.GetComponent<BoxCollider>().center += new Vector3(0f, 0f, originPos.x + originPos.z);
        }
        newBlock.transform.position += offsetPos;
        newBlock.transform.eulerAngles = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        gridData.SetPosition(new Vector3(buildPos.x + offsetPos.x, buildPos.y + offsetPos.y, buildPos.z + offsetPos.z));
        if (ValueEquals(GridCeil(gridData.cellX), GridCeil(gridData.cellY), GridCeil(gridData.cellHeight)))
        {
            repeatedState = true;
            if (gridType == GridType.Block)
            {
                gridManager.UpdateToken(cost);
                if(gridData.isDefense)
                {
                    gridManager.AddDefense(1);
                }
                Destroy(newBlock);
                
            }
            if (gridType == GridType.Structure)
            {
                newBlock.transform.position = Vector3.zero;
                gridData.reset();
                tokenManager.addTokens(cost);
                newBlock.SetActive(false);
            }
            return;
        }
        gridCells.Add(new GridCell(GridCeil(buildPos.x + offsetPos.x), GridCeil(buildPos.z + offsetPos.z), (int)(buildPos.y + offsetPos.y)));
        if (gridData != null && gridType == GridType.Block)
        {
            gridData.Rotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
            gridData.Scale = gridManager.PlaceBlockObject.transform.localScale;
            gridData.blockId = gridManager.CurrentBlockId;
            gridData.isDefense = blockSO.blockData[gridManager.CurrentBlockId].isDefense;
            gridData.isUtility = blockSO.blockData[gridManager.CurrentBlockId].isUtility;
            gridData.tokenCost = blockSO.blockData[gridManager.CurrentBlockId].tokenCost;
            gridData.gridSize = gridSize;
        }
        newBlock.tag = "Grid";
        newBlock.layer = LayerMask.NameToLayer("Grid");
        MeshRenderer meshRenderer = newBlock.GetComponent<MeshRenderer>();
        if (meshRenderer != null && gridType != GridType.Structure)
        {
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        hitStructure = false;
    }
    private GameObject targetBlock;
    private Transform parent;
    private Transform shootingPoint;
    float range;
    private bool isBreak;
    private Vector3 GetOffsetPosFromDir(Vector3 direction, Vector3 originalOffset)
    {
        //Debug.Log("Direction: " + direction) ;
        Vector3 newOffset = Vector3.zero;
        if (direction == Vector3.zero)
        {
            newOffset.z -= originalOffset.z;
        }
        else if (direction.y == 90f)
        {
            newOffset.x -= originalOffset.x;
        }
        else if (direction.y == -90f)
        {
            newOffset.x += originalOffset.x;
        }
        else if (direction.y == 180f)
        {
            newOffset.z += originalOffset.z;
        }
        newOffset.y = originalOffset.y;
        return newOffset;
    }
    public void DestroyBlock(Transform shootingPoint, float range)
    {
        RaycastHit hit;
        if (lastOutlineBlock != null)
        {
            hitStructure = false;
            GridData gridData = lastOutlineBlock.transform.GetComponent<GridData>();
            if (gridData != null && gridType == GridType.Block)
            {
                gridManager.UpdateToken(gridData.tokenCost);
                if(gridData.isDefense)
                {
                    gridManager.AddDefense(-1);
                }
               
                Destroy(lastOutlineBlock.transform.gameObject);
            }
            if(gridType == GridType.Structure)
            {
                tokenManager.addTokens(gridData.tokenCost);
                lastOutlineBlock.transform.position = Vector3.zero;
                lastOutlineBlock.transform.gameObject.SetActive(false);
            }
            if (gridData != null)
            {
                RemoveValue(GridCeil(gridData.cellX), GridCeil(gridData.cellY), (int)gridData.cellHeight);
            }
            return;
        }

    }

    public Quaternion GetDirection(Ray ray)
    {
        Vector3 rayDirection = -ray.direction;
        rayDirection.y = 0f;
        float angle = Mathf.Atan2(rayDirection.z, rayDirection.x) * Mathf.Rad2Deg;
        float roundedAngle = Mathf.Round(angle / 45f) * 45f;
        float roundedAngleRad = roundedAngle * Mathf.Deg2Rad;
        Vector3 rotatedDirection = new Vector3(Mathf.Cos(roundedAngleRad), 0f, Mathf.Sin(roundedAngleRad));
        return Quaternion.LookRotation(rotatedDirection);
    }
    bool boundState;
    private bool distanceState;
    private bool repeatedState;
    private bool reachMaxStructure;
    private bool rayhitStructure = false;
    private bool hitStructure = false;
    private void ErrorMessage()
    {

        InventoryBehaviour inventoryBehaviour = userObject.GetComponent<InventoryBehaviour>();
        if (distanceState == false)
        {
            if(inventoryBehaviour != null)
            inventoryBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Too close to build a block", Color.red);
            else if(displayBehaviour!=null)
            {
                displayBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Too close to build a block", Color.red);
            }
        }
        if (boundState == false)
        {
            if(inventoryBehaviour != null)
            inventoryBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Block out of bounds", Color.red);
            else if (displayBehaviour != null)
            {
                displayBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Block out of bounds", Color.red);
            }

        }
        if(repeatedState)
        {
            if(inventoryBehaviour != null)
            inventoryBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Block Occupied", Color.red);
            else if(displayBehaviour != null)
            {
                displayBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Block Occupied!", Color.red);
            }
            repeatedState = false;
        }
        if(reachMaxStructure)
        {
            if (inventoryBehaviour != null)
                inventoryBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Max repeated Structures reached! (10)", Color.red);
            else
                if (displayBehaviour != null)
                {
                    displayBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Max repeated Structures reached! (10)", Color.red);
                }
        }
        if(gridType == GridType.Structure && structureManager.CurrentStructure == -1)
        {
            if (inventoryBehaviour != null)
                inventoryBehaviour.StartFadeInText("There is no " + gridType.ToString() + " for you to place!", Color.white);
            else
                  if (displayBehaviour != null)
            {
                displayBehaviour.StartFadeInText("There is no " + gridType.ToString() + " for you to place!", Color.white);
            }

        }
        //if(rayhitStructure)
        //{
        //    if (inventoryBehaviour != null)
        //        inventoryBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Pointing to a object", Color.red);
        //    else
        //    if (displayBehaviour != null)
        //    {
        //        displayBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Pointing to a object", Color.red);
        //    }

        //}
        if(hitStructure && templateGameobject != null)
        {
            if (inventoryBehaviour != null)
                inventoryBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Colliding with another " + gridType.ToString(), Color.red);
            else
            if (displayBehaviour != null)
            {
                displayBehaviour.StartFadeInText("Unable to place " + gridType.ToString() + ". Reason: Colliding with another " + gridType.ToString(), Color.red);
            }

        }


    }
    private bool IsValidPosition(Vector3 spawnPosition)
    {
        float distance = Vector3.Distance(playerCamera.transform.position, spawnPosition);
        boundState = spawnPosition.x > -numRows / 2f && spawnPosition.x < numRows / 2f &&
               spawnPosition.y >= 0 && spawnPosition.y < numHeight &&
               spawnPosition.z > -numColumns / 2f && spawnPosition.z < numColumns / 2f;
        distanceState = distance >= minBuildDistance;
       if(gridType == GridType.Structure)
        {
            rayhitStructure = lastOutlineBlock == null;
        }
        // Check if the spawn position is within the valid range
        return boundState && distanceState && !repeatedState;
    }
    GameObject lastOutlineBlock;
    private void PerformOutlineRaycast()
    {
        GameObject newHitBlock = null;
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hit, maxBuildDistance, buildableSurfacesLayer))
        {

            newHitBlock = hit.collider.transform.gameObject;
            if(newHitBlock.GetComponent<GridData>() == null)
            {
                switch (gridType)
                {
                    case GridType.Block:
                        if(lastOutlineBlock != null)
                            setOutline(lastOutlineBlock, false);
                        break;
                    case GridType.Structure:
                        if (lastOutlineBlock != null)
                        {
                            //MaterialBehaviour matBehNew = lastOutlineBlock.GetComponent<MaterialBehaviour>();
                            //if (matBehNew != null)
                            //{
                            //    matBehNew.ResetMat();
                            //}
                            setOutline(lastOutlineBlock, false);
                        }
                        break;
                }
                lastOutlineBlock = null;
                return;
            }
        } else
        {
            newHitBlock = null;
        }

        if (newHitBlock != lastOutlineBlock)
        {

            switch (gridType)
            {
                case GridType.Block:
                    if (lastOutlineBlock != null)
                    {
                        setOutline(lastOutlineBlock, false);
                    }
                    if (newHitBlock != null)
                    {
                        setOutline(newHitBlock, true);
                    }
                    break;
                case GridType.Structure:
                    if (lastOutlineBlock != null)
                    {
                        //MaterialBehaviour matBehOrigin = lastOutlineBlock.GetComponent<MaterialBehaviour>();
                        //if (matBehOrigin == null) { matBehOrigin = lastOutlineBlock.AddComponent<MaterialBehaviour>(); }
                        //matBehOrigin.ResetMat();
                        setOutline(lastOutlineBlock, false);
                    }
                    if (newHitBlock != null)
                    {
                        //MaterialBehaviour matBehNew = newHitBlock.GetComponent<MaterialBehaviour>();
                        //if(matBehNew == null) { matBehNew = newHitBlock.AddComponent<MaterialBehaviour>(); }
                        //matBehNew.SetMaterial(highlightMaterial);
                        setOutline(newHitBlock, true);
                    }
                    break;

            }
            lastOutlineBlock = newHitBlock;
        }
        else if (lastOutlineBlock == null && newHitBlock != null)
        {
            switch(gridType) 
            {
                case GridType.Block:
                    setOutline(newHitBlock, true);
                    break;
                case GridType.Structure:
                    //MaterialBehaviour matBehNew = newHitBlock.GetComponent<MaterialBehaviour>();
                    //if (matBehNew == null) { matBehNew = newHitBlock.AddComponent<MaterialBehaviour>(); }
                    //matBehNew.SetMaterial(highlightMaterial);
                    setOutline(newHitBlock, true);
                    break;
            }
            lastOutlineBlock = newHitBlock;

        }
    }
    private void setOutline(GameObject target, bool state)
    {
        GridData gridData = target.transform.GetComponent<GridData>();
        if (gridData == null) return;
        Outline outline = target.GetComponent<Outline>();
        if (outline == null && target != null)
        {
            outline = target.AddComponent<Outline>();

        }
        if (lastOutlineBlock != null)
        {
            Outline outline1 = lastOutlineBlock.GetComponent<Outline>();
            if(outline1!=null)
            {
                outline1.OutlineWidth = 0f;
            }
        }

        if (state)
        {
            outline.OutlineColor = Color.white;
            lastOutlineBlock = target;
            outline.OutlineWidth = 10f;
            if (gridData.isUtility == true)
            {
                switch (gridData.originInteractType)
                {
                    case InteractType.Body:
                        if (gridData.originGameObjectId == -1 && gridData.id >= 0)
                        {
                            outline.OutlineColor = Color.cyan;
                        }
                        else
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
            if(gridData.isUtility == true)
            {
                outline.OutlineColor = Color.blue;
            }
        }
        else
        {
            outline.OutlineWidth = 0f;
        }
    }
    public static int GridCeil(float value)
    {
        if (value >= 0)
        {
            return Mathf.CeilToInt(value);
        }
        else
        {
            return Mathf.FloorToInt(value);
        }
    }
}

public class BlockSystem : MonoBehaviour
{

    [SerializeField]
    private BlockType[] allBlockTypes;

    [HideInInspector]
    public Dictionary<int, Block> allBlocks = new Dictionary<int, Block>();

    private void Awake()
    {
        for (int i = 0; i < allBlockTypes.Length; i++)
        {
            BlockType newBlockType = allBlockTypes[i];
            Block newBlock = new Block(i, newBlockType.blockName, newBlockType.blockMat);
            allBlocks[i] = newBlock;
            Debug.Log("Block added to dictionary " + allBlocks[i].blockName);
        }
    }
}

public class Block
{
    public int blockID;
    public string blockName;
    public Material blockMaterial;

    public Block(int id, string name, Material mat)
    {
        blockID = id;
        blockName = name;
        blockMaterial = mat;
    }
}

[Serializable]
public struct BlockType
{
    public string blockName;
    public Material blockMat;
}
public enum GridType
{
    Block,
    Structure
}
public enum TemplateType
{
    Outline,
    GameObject
}