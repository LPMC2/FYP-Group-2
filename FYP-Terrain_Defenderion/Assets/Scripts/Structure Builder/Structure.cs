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
}