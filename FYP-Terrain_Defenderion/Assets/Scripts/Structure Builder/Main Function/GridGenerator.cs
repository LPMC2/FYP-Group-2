using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private Camera playerCamera;

    private bool buildModeOn = false;
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
                Debug.Log(buildPos);
                if (IsValidPosition(buildPos))
                    canBuild = true;
                else
                    canBuild = false;
            }
            else
            {
                canBuild = false;
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
            if(Input.GetMouseButtonDown(0))
            {
                DestroyBlock(shootingPoint, range);
            }
        }
    }
    private void PlaceBlock()
    {
        if (!gridManager.isTokenAffordable(TokenManager.GetTokenCost(gridManager.CurrentBlockId), null))
        {
            return;
        }
        if (gridManager.IsMaxDefenseReached(gridManager.CurrentBlockId))
        {
            return;
        }
        Ray ray = new Ray(shootingPoint.position, shootingPoint.forward * range);
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
        newBlock.transform.eulerAngles = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        gridData.SetPosition(buildPos);
        gridData.Rotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
        gridData.blockId = gridManager.CurrentBlockId;
        gridData.isDefense = blockSO.blockData[gridManager.CurrentBlockId].isDefense;
        gridData.isUtility = blockSO.blockData[gridManager.CurrentBlockId].isUtility;
        gridData.tokenCost = blockSO.blockData[gridManager.CurrentBlockId].tokenCost;
        newBlock.tag = "Grid";
        newBlock.layer = LayerMask.NameToLayer("Grid");
        Debug.Log(newBlock.layer);
    }
    private GameObject targetBlock;
    private Transform parent;
    private Transform shootingPoint;
    float range;
    private bool isBreak;
    //public GameObject BuildBlock(GameObject block, Transform parent, Transform shootingPoint, float range)
    //{
    //    Ray ray = new Ray(shootingPoint.position, shootingPoint.forward * range);
    //    GameObject targetBlock = null;
    //    if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, range))
    //    {
    //        Debug.DrawRay(shootingPoint.position, shootingPoint.forward * range, Color.magenta, 5f);
    //        Vector3 spawnPosition = Vector3.zero;
    //        if (hitInfo.transform.CompareTag("Grid"))
    //        {
    //            spawnPosition = GetNearestPointOnGrid(hitInfo.point, shootingPoint, hitInfo.normal);
    //            if (IsValidPosition(spawnPosition))
    //            {
    //               targetBlock = Instantiate(block, spawnPosition, Quaternion.identity, parent);
    //                GridData gridData = targetBlock.AddComponent<GridData>();
    //                gridData.SetPosition(spawnPosition);
    //            }
    //        }
    //        else if(hitInfo.transform.CompareTag("GridPlane"))
    //        {
    //            spawnPosition = GetNearestPointOnGrid(hitInfo.point, shootingPoint, Vector3.zero);
    //            Vector3 sPointLocation = new Vector3(shootingPoint.parent.localPosition.x, shootingPoint.parent.localPosition.y, shootingPoint.parent.localPosition.z);
    //            Debug.Log(spawnPosition + "\n" + spawnPosition + shootingPoint.parent.localPosition);
    //                targetBlock = Instantiate(block, spawnPosition , Quaternion.identity, parent);
    //            GridData gridData = targetBlock.AddComponent<GridData>();
    //            gridData.cellX = (int)(spawnPosition.x / cellSize * 2);
    //                gridData.cellY = (int)(spawnPosition.z / cellSize * 2);
    //                gridData.cellHeight = (int)(spawnPosition.y / cellSize * 2);

    //        }
    //        if(targetBlock != null)
    //        {
    //            Quaternion direction = GetDirection(ray);
    //            GridData gridData = targetBlock.GetComponent<GridData>();
    //            Vector3 eulerRotation = direction.eulerAngles;
    //            targetBlock.transform.localScale = new Vector3(cellSize, cellSize, cellSize);
    //            eulerRotation.x = GridManager.NormalizeAngle(eulerRotation.x);
    //            eulerRotation.y = GridManager.NormalizeAngle(eulerRotation.y);
    //            eulerRotation.z = GridManager.NormalizeAngle(eulerRotation.z);
    //            targetBlock.transform.eulerAngles = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
    //            gridData.Rotation = new Vector3(eulerRotation.x, eulerRotation.y, eulerRotation.z);
    //            gridData.blockId = blockID;
    //            gridData.isDefense = blockSO.blockData[blockID].isDefense;
    //            gridData.isUtility = blockSO.blockData[blockID].isUtility;
    //            targetBlock.tag = "Grid";
    //            targetBlock.layer = LayerMask.NameToLayer("Grid");
    //        }
    //    }
    //    return targetBlock;
    //}

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

    //private static Vector3 CalculateSpawnPosition(Vector3 hitPoint, Vector3 hitNormal, float cellSize)
    //{
    //    Debug.Log(hitPoint + "/" + hitNormal);
    //    float x =(hitPoint.x + hitNormal.x)  ;
    //    float y = (hitPoint.y + hitNormal.y)   ;
    //    float z = (hitPoint.z + hitNormal.z)   ;

    //    return new Vector3(x, y, z);
    //}
    //public Vector3 GetNearestPointOnGrid(Vector3 position, Transform placerPos, Vector3 hitNormal)
    //{
    //    position -= placerPos.position;

    //    int xCount = Mathf.RoundToInt(position.x / cellSize);
    //    int yCount = Mathf.RoundToInt(position.y / cellSize);
    //    int zCount = Mathf.RoundToInt(position.z / cellSize);

    //    Vector3 result = new Vector3(
    //        (float)xCount * cellSize,
    //        (float)yCount * cellSize,
    //        (float)zCount * cellSize);
    //    if(hitNormal.y == 0f)
    //    {
    //        result += hitNormal * cellSize;
    //    }
    //    result += placerPos.position;

    //    return result;
    //}
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
    private bool IsValidPosition(Vector3 spawnPosition)
    {

        // Check if the spawn position is within the valid range
        return spawnPosition.x > -numRows / 2f && spawnPosition.x < numRows / 2f &&
               spawnPosition.y >= 0 && spawnPosition.y < numHeight &&
               spawnPosition.z > -numColumns/2f && spawnPosition.z < numColumns/2f;
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