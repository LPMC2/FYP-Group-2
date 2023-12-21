using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private float minBuildDistance = 1f;
    private bool buildModeOn = false;
    public bool BuildMode { get { return buildModeOn; } set { buildModeOn = value; } }
    private bool canBuild = false;

    private BlockSO bSys;

    [SerializeField]
    private LayerMask buildableSurfacesLayer;

    private Vector3 buildPos;

    private GameObject currentTemplateBlock;

    [SerializeField]
    private GameObject blockTemplatePrefab;
    [SerializeField]
    private GameObject blockPrefab;

    [SerializeField]
    private Material templateMaterial;

    private int blockSelectCounter = 0;

    private void Start()
    {

        buildModeOn = true;
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
    private void Update()
    {
       
        if (buildModeOn)
        {
            RaycastHit buildPosHit;

            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out buildPosHit, 10, buildableSurfacesLayer))
            {
                Vector3 point = buildPosHit.point;
                buildPos = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), Mathf.Round(point.z));

                if (IsValidPosition(buildPos))
                    canBuild = true;
                else
                    canBuild = false;
            }
            else
            {
                canBuild = false;
            }
            if (Input.GetMouseButtonDown(0))
            {
                DestroyBlock(shootingPoint, range);
            }
        }

        if (!buildModeOn)
        {
        
            canBuild = false;
        }

        if (canBuild)
        {
            PerformOutlineRaycast();

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
    }
    private void PlaceBlock()
    {
        if (gridManager.PlaceBlockObject == null || gridManager.CurrentBlockId == -1) return;
        if (!gridManager.isTokenAffordable(TokenManager.GetTokenCost(gridManager.CurrentBlockId), null))
        {
            return;
        } else if(gridManager.IsMaxDefenseReached(gridManager.CurrentBlockId))
        {
            return;
        }

        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward);
        GameObject newBlock = Instantiate(gridManager.PlaceBlockObject, buildPos, Quaternion.identity, parent);
        BoxCollider boxCollider = newBlock.GetComponent<BoxCollider>();
        if(boxCollider != null)
        {
            boxCollider.size += new Vector3(0.0001f, 0.0001f, 0.0001f);
        }
        Quaternion direction = GetDirection(ray);
        GridData gridData = newBlock.AddComponent<GridData>();
        Vector3 eulerRotation = direction.eulerAngles;
        //newBlock.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
        eulerRotation.x = GridManager.NormalizeAngle(eulerRotation.x);
        eulerRotation.y = GridManager.NormalizeAngle(eulerRotation.y);
        eulerRotation.z = GridManager.NormalizeAngle(eulerRotation.z);
        Vector3 offsetPos = GetOffsetPosFromDir(eulerRotation ,gridManager.PlaceBlockObject.transform.position);
        if(newBlock.GetComponent<BoxCollider>()!=null)
        newBlock.GetComponent<BoxCollider>().center += new Vector3(0f, 0f, gridManager.PlaceBlockObject.transform.position.x + gridManager.PlaceBlockObject.transform.position.z);
        newBlock.transform.position += offsetPos;
        newBlock.transform.eulerAngles = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        gridData.SetPosition(buildPos+offsetPos);
        gridData.Rotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        gridData.Scale = gridManager.PlaceBlockObject.transform.localScale;
        gridData.blockId = gridManager.CurrentBlockId;
        gridData.isDefense = blockSO.blockData[gridManager.CurrentBlockId].isDefense;
        gridData.isUtility = blockSO.blockData[gridManager.CurrentBlockId].isUtility;
        gridData.tokenCost = blockSO.blockData[gridManager.CurrentBlockId].tokenCost;
        newBlock.tag = "Grid";
        newBlock.layer = LayerMask.NameToLayer("Grid");
        MeshRenderer meshRenderer = newBlock.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
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
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hit, range, buildableSurfacesLayer))
        {
            GridData gridData = hit.transform.GetComponent<GridData>();
            if (gridData != null)
            {
                gridManager.UpdateToken(gridData.tokenCost);
                if(gridData.isDefense)
                {
                    gridManager.AddDefense(-1);
                }
                Destroy(hit.transform.gameObject);
            }
            return;
        }

    }

    public Quaternion GetDirection(Ray ray)
    {
        Vector3 rayDirection = -ray.direction;
        rayDirection.y = 0f;
        float angle = Mathf.Atan2(rayDirection.z, rayDirection.x) * Mathf.Rad2Deg;
        float roundedAngle = Mathf.Round(angle / 90f) * 90f;
        float roundedAngleRad = roundedAngle * Mathf.Deg2Rad;
        Vector3 rotatedDirection = new Vector3(Mathf.Cos(roundedAngleRad), 0f, Mathf.Sin(roundedAngleRad));
        return Quaternion.LookRotation(rotatedDirection);
    }
    bool boundState;
    private bool distanceState;
    private void ErrorMessage()
    {

        InventoryBehaviour inventoryBehaviour = gameObject.GetComponent<InventoryBehaviour>();
        if (inventoryBehaviour == null) return;
        if (distanceState == false)
        {
            inventoryBehaviour.StartFadeInText("Unable to place block. Reason: Too close to build a block", Color.red);
        }
        if (boundState == false)
        {
            inventoryBehaviour.StartFadeInText("Unable to place block. Reason: Block out of bounds", Color.red);
               
        }

        
    }
    private bool IsValidPosition(Vector3 spawnPosition)
    {
        float distance = Vector3.Distance(playerCamera.transform.position, spawnPosition);
        boundState = spawnPosition.x > -numRows / 2f && spawnPosition.x < numRows / 2f &&
               spawnPosition.y >= 0 && spawnPosition.y < numHeight &&
               spawnPosition.z > -numColumns / 2f && spawnPosition.z < numColumns / 2f;
        distanceState = distance >= minBuildDistance;
        
        // Check if the spawn position is within the valid range
        return spawnPosition.x > -numRows / 2f && spawnPosition.x < numRows / 2f &&
               spawnPosition.y >= 0 && spawnPosition.y < numHeight &&
               spawnPosition.z > -numColumns/2f && spawnPosition.z < numColumns/2f && distance >= minBuildDistance;
    }
    GameObject lastOutlineBlock;
    private void PerformOutlineRaycast()
    {
        GameObject newHitBlock = null;
        RaycastHit hit;
        if(Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hit, range * range, buildableSurfacesLayer))
        {

            newHitBlock = hit.collider.transform.gameObject;
        } else
        {
            newHitBlock = null;
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
        }
        else if (lastOutlineBlock == null && newHitBlock != null)
        {
            setOutline(newHitBlock, true);
            lastOutlineBlock = newHitBlock;
        }
    }
    private void setOutline(GameObject target, bool state)
    {
        GridData gridData = target.transform.GetComponent<GridData>();
        if (gridData == null) return;
        Outline outline = target.GetComponent<Outline>();
        if (outline == null)
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