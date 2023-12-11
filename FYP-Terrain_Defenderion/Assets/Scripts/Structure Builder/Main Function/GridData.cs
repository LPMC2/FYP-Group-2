using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
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
        isDefense = structureStorage.isDefense;
    }

}
public enum InteractType
{
    none,
    Body,
    Head
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