using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
public class GridData : MonoBehaviour
{
    public GridSize gridSize;
    public float cellX = 0;
    public float cellY = 0;
    public float cellHeight = 0;
    public int blockId = -1;
    public Vector3 Rotation;
    public Vector3 Scale = new Vector3(1,1,1);
    public bool isAutoRotatable = true;
    public int tokenCost = 0;
    public int id = -1;
    public InteractType originInteractType = default;
    public int originGameObjectId = -1;
    public bool isUtility = false;
    public bool isDefense = false;
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
        isDefense = false;
        isUtility = false;
    }
    public void SetData(int BlockID, Vector3 pos, Vector3 rotation, Vector3 scale, int TokenCost, int ID, InteractType type, int originID, bool isUtility, bool isDefense, GridSize gridSize)
    {
        blockId = BlockID;
        SetPosition(pos);
        Rotation = rotation;
        Scale = scale;
        tokenCost = TokenCost;
        id = ID;
        originInteractType = type;
        originGameObjectId = originID;
        this.isUtility = isUtility;
        this.isDefense = isDefense;
        this.gridSize = gridSize;
    }
    public void SetData(StructureStorage structureStorage)
    {
        cellX = structureStorage.cellPos[0];
        cellY = structureStorage.cellPos[2];
        cellHeight = structureStorage.cellPos[1];
        Rotation = new Vector3(structureStorage.Rotation[0], structureStorage.Rotation[1], structureStorage.Rotation[2]);
        Scale = new Vector3(structureStorage.Scale[0], structureStorage.Scale[1], structureStorage.Scale[2]);
        tokenCost = structureStorage.tokenCost;
        isAutoRotatable = structureStorage.isAutoRotatable;
        id = structureStorage.id;
        originInteractType = structureStorage.originInteractType;
        originGameObjectId = structureStorage.originGameObjectId;
        isUtility = structureStorage.isUtility;
        isDefense = structureStorage.isDefense;
        gridSize = structureStorage.gridSize;
    }
    public void SetPosition(Vector3 position)
    {
        cellX = position.x;
        cellY = position.z;
        cellHeight = position.y;
    }
}
public enum InteractType
{
    none,
    Body,
    Head,
    Wall
}

public class DebugScript
{
    public static void DebugVariables<T>(T component)
        where T : Component
    {
        string debugLog = component.name + ": \n";
        FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach(FieldInfo field in fields)
        {
            string fieldName = field.Name;
            object value = field.GetValue(component);
            debugLog += fieldName + ":" + value + "\n";
        }
        Debug.Log(debugLog, component.gameObject);

    }
}