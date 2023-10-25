using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData : MonoBehaviour
{
    public int cellX = 0;
    public int cellY = 0;
    public int cellHeight = 0;
    public int blockId = -1;
    public Vector3 Rotation;
    public Vector3 Scale = new Vector3(1,1,1);
    public bool isAutoRotatable = true;
    public int tokenCost = 0;
    public int id = -1;
    public InteractType originInteractType = default;
    public int originGameObjectId = -1;
    public bool isUtility = false;
    public void reset()
    {
        blockId = -1;
        Rotation = Vector3.zero;
        Scale = Vector3.one;
        isAutoRotatable = false;
        tokenCost = 0;
        id = -1;
        originInteractType = default;
        originGameObjectId = -1;
        
        isUtility = false;
    }
    public void SetData(StructureStorage structureStorage)
    {
        cellX = (int)structureStorage.cellPos[0];
        cellY = (int)structureStorage.cellPos[2];
        cellHeight = (int)structureStorage.cellPos[1];
        Rotation = new Vector3(structureStorage.Rotation[0], structureStorage.Rotation[1], structureStorage.Rotation[2]);
        Scale = new Vector3(structureStorage.Scale[0], structureStorage.Scale[1], structureStorage.Scale[2]);
        tokenCost = structureStorage.tokenCost;
        isAutoRotatable = structureStorage.isAutoRotatable;
        id = structureStorage.id;
        originInteractType = structureStorage.originInteractType;
        originGameObjectId = structureStorage.originGameObjectId;
        isUtility = structureStorage.isUtility;
    }
}
public enum InteractType
{
    none,
    Body,
    Head
}