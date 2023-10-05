using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData
{

    public GameObject blockModel;
    [Header("Note: Block texture only applies when no block model found")]
    public Material blockTexture;
    public int maxHealth = 1;
    public int tokenCost = 1;
    public Material getBlockTexture()
    {
        return blockTexture;
    }

}
