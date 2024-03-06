using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockData : Item
{

    [Header("Block Selection")]
    [Tooltip("Note: Block texture only applies when no block model found")]
    public Material blockTexture;
    public int maxHealth = 1;
    public int tokenCost = 1;
    public float captureOrthographicSize = 1f;
    [Header("Utility Type Selection")]
    public bool isUtility = false;
    public bool isDefense = false;
    public InteractType utilityType = default;
    public Material getBlockTexture()
    {
        return blockTexture;
    }

}
