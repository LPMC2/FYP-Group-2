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
    public bool isAutoRotatable;

    public void reset()
    {
       cellX = 0;
       cellY = 0;
       cellHeight = 0;
       blockId = -1;
       Rotation = Vector3.zero;
       Scale = Vector3.zero;
       isAutoRotatable = false;
}
}
