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
}