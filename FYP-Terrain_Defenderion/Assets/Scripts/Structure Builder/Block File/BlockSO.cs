using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockStorage", menuName = "ScriptableObjects/BlockSO")]
public class BlockSO : ScriptableObject
{
    public BlockData[] blockData;
    public int id = 0;

    public int GetId()
    {
        id++;
        return id;
    }
}
