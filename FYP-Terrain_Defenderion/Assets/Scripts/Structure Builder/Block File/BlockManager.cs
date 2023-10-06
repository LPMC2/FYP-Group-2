using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BlockManager
{
    private static BlockSO blockData;

    public static BlockSO BlockData
    {
        get
        {
            if (blockData == null)
            {
                blockData = Resources.Load<BlockSO>("BlockData/Data");

                if (blockData == null)
                {
                    Debug.LogError("BlockData not found!");
                }
            }

            return blockData;
        }
    }
}