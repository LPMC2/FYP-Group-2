using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public int structureId { get; private set; }
    public float[] cellPos { get; private set; }
    public float[] Rotation { get; private set; }
    public float[] Scale { get; private set; }
    public bool isAutoRotatable { get; private set; }
    public int tokenCost { get; private set; }
    public int id { get; private set; }
    public InteractType originInteractType { get; private set; }
    public int originGameObjectId { get; private set; }
    public bool isUtility { get; private set; }
}
[System.Serializable]
public class StructureStorage
{
    public GridSize gridSize;
    public int structureId;
    public float[] cellPos = new float[3];
    public float[] Rotation = new float[3];
    public float[] Scale = new float[3];
    public bool isAutoRotatable = true;
    public int tokenCost = 0;
    public int id = -1;
    public InteractType originInteractType = default;
    public int originGameObjectId = -1;
    public bool isUtility = false;
    public bool isDefense = false;
    public StructureStorage ()
    {
        //structureId = structure.structureId;
        //cellPos = structure.cellPos;
        //Rotation = structure.Rotation;
        //Scale = structure.Scale;
        //isAutoRotatable = structure.isAutoRotatable;
    }
    public static GameObject[] GetOriginBodyFromId(StructureStorage[] structureStorages, GameObject targetParent, InteractType type)
    {
        BlockSO blockData = BlockManager.BlockData;
        //Find Origin of Body Objects
        GameObject[] bodyStructure = new GameObject[0];
        GameObject[] headStructure = new GameObject[0];
        int count = 0;
        if (type == InteractType.Body)
        {
            //Find Origin of Body Objects
            foreach (StructureStorage structureStorage in structureStorages)
            {
                if (structureStorage.id > -1 && structureStorage.originInteractType == InteractType.Body)
                {
                    foreach(Transform child in targetParent.transform)
                    {
                        GridData gridData = child.GetComponent<GridData>();
                        if(gridData.id != -1 && gridData.id == structureStorage.id && structureStorage.originInteractType == gridData.originInteractType)
                        {
                            bodyStructure = arrayBehaviour.AddArray<GameObject>(bodyStructure);
                            bodyStructure[count] = child.gameObject;
                            count++;

                        }
                    }

                }
            }
            return bodyStructure;
        }
        count = 0;
        if (type == InteractType.Head)
        {
            foreach (StructureStorage structureStorage in structureStorages)
            {
                if (structureStorage.id > -1 && structureStorage.originInteractType == InteractType.Head)
                {
                    
                    foreach (Transform child in targetParent.transform)
                    {
                        GridData gridData = child.GetComponent<GridData>();
                        Debug.Log(gridData.id + " " + gridData.id + " " + structureStorage.id + " " + structureStorage.originInteractType + " " + gridData.originInteractType);
                        if (gridData.id != -1 && gridData.id == structureStorage.id && structureStorage.originInteractType == gridData.originInteractType)
                        {
                            headStructure = arrayBehaviour.AddArray<GameObject>(headStructure);
                            headStructure[count] = child.gameObject;
                            count++;

                        }
                    }
                }
            }
            return headStructure;
        }

        return null;
    }
    public static void SetParentFromStructureId(StructureStorage[] structureStorages, GameObject containerGameObject, InteractType utilityType)
    {
        GameObject[] idObjects = GetOriginBodyFromId(structureStorages, containerGameObject, utilityType);
        for (int i = 0; i < idObjects.Length; i++)
        {
            Debug.Log("Object: " + idObjects[i]);
            GridData originGridData = idObjects[i].GetComponent<GridData>();
            foreach (Transform child in containerGameObject.transform)
            {
                GridData gridData = child.GetComponent<GridData>();
                switch (utilityType)
                {
                    case InteractType.Head:
                    case InteractType.Body:
                        if(gridData.id == -1 && gridData.originGameObjectId == originGridData.id)
                        {
                            child.SetParent(idObjects[i].transform);
                        }
                        break;
                }
            }
        }
    }
    public static void ProcessChildObjects(Transform parentObject)
    {
        GameObject[,] bodyList = new GameObject[0, 1];
        GameObject[,] headList = new GameObject[0, 1];
        GameObject other = null;
        int count = 0;
        foreach (Transform target in parentObject)
        {
            GridData gridData = target.GetComponent<GridData>();

            if(gridData.id > -1)
            {

                switch(gridData.originInteractType) 
                {
                    case InteractType.Body:
                        bodyList= arrayBehaviour.Add2DArray(bodyList, ArrayType.row);
                        bodyList[bodyList.Length-1, 0] = target.gameObject;
                        break;
                    case InteractType.Head:
                        headList= arrayBehaviour.Add2DArray(headList, ArrayType.row);
                        headList[headList.Length-1, 0] = target.gameObject;
                        break;
                }
                count++;
            }
        }

        Debug.Log(count);
         count = 0;
        foreach (Transform child in parentObject)
        {
            count++;
            GridData gridData = child.GetComponent<GridData>();
            DebugScript.DebugVariables(gridData);
            if (gridData != null && gridData.id > -1 && gridData.originGameObjectId == -1)
            {
                // Skip this child object
                continue;
            }
            //Find childs of body/head object

            if (gridData != null && gridData.originGameObjectId > -1)
            {
                GameObject parent = default;
                switch (gridData.originInteractType)
                {
                    case InteractType.Body:
                        bodyList = arrayBehaviour.Add2DArray(bodyList, ArrayType.column);
                        int targetId = FindParentId(bodyList, gridData.originGameObjectId);
                        bodyList[targetId, bodyList.GetLength(1)-1] = child.gameObject;
                        break;
                    case InteractType.Head:
                        headList = arrayBehaviour.Add2DArray(headList, ArrayType.column);
                        int targetId1 = FindParentId(headList, gridData.originGameObjectId);
                        bodyList[targetId1, bodyList.GetLength(1) - 1] = child.gameObject;
                        break;
                }

            }

            arrayBehaviour.Debug2DArray(bodyList, nameof(bodyList));
            arrayBehaviour.Debug2DArray(headList, nameof(headList));
            //if (gridData != null && gridData.id == -1 && gridData.originGameObjectId > -1)
            //{
            //    switch (gridData.originInteractType)
            //    {
            //        case InteractType.Body:

            //            break;
            //        case InteractType.Head:

            //            break;
            //    }
            //}

            if (other == null && gridData.originInteractType == default || gridData.isUtility == false)
            {
                if (other == null)
                {
                    other = new GameObject("other");
                    other.transform.SetParent(parentObject);
                }
                child.SetParent(other.transform);
            }
            SetParentFromType(headList);
            SetParentFromType(bodyList);

        }
        Debug.Log("Number: " + count);
    }
    private static void SetParentFromType(GameObject[,] list)
    {
       for(int i=0; i< list.Length; i++)
        {
            for (int j= 1;j<list.GetLength(1); j++)
            {
                list[i, 0].transform.SetParent(list[i, j].transform);

            }
        }

    }

    private static GameObject FindParentObject(List<GameObject> objects, int originGameObjectId)
    {
        foreach (GameObject obj in objects)
        {
            GridData gridData = obj.GetComponent<GridData>();
            if (gridData != null && gridData.id == originGameObjectId)
            {
                return obj;
            }
        }
        return null;
    }
    private static int FindParentId(GameObject[,] target, int originGameObjectId)
    {
        int targetId = -1;
        for(int i=0; i< target.GetLength(0); i++)
        {
            Debug.Log("Array: " + i);
            if(target[i,0].GetComponent<GridData>().id == originGameObjectId)
            {
                targetId = i;
            }
        }
        return targetId;
    }
}