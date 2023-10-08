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
}
[System.Serializable]
public class StructureStorage
{

    public int structureId;
    public float[] cellPos = new float[3];
    public float[] Rotation = new float[3];
    public float[] Scale = new float[3];
    public bool isAutoRotatable;
    public int tokenCost = 0;
    public StructureStorage ()
    {
        //structureId = structure.structureId;
        //cellPos = structure.cellPos;
        //Rotation = structure.Rotation;
        //Scale = structure.Scale;
        //isAutoRotatable = structure.isAutoRotatable;
    }
}