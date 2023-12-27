using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="InvBagSO", menuName = "Inventory/Create InventoryBag")]
public class InventoryBagSO : ScriptableObject
{
    public InvType[] inventoryBag;
    public BlockSO itemSO;
    [Header("UI Settings")]
    public GameObject typeBtnPrefab;
    public GameObject MenuNamePrefab;
    public GameObject MenuSlotPrefab;

    public int GetIndexFromTypeName(string name)
    {
        int index = 0;
        foreach(InvType targetname in inventoryBag)
        {
            if(name == targetname.TypeName)
            {
                return index;
            }
            index++;
        }
        return -1;
    }
    public int[] SetItemSize(InvMenu target, int size, bool ignoreOriginValue = false, bool isList = false, int originIndexSize = 0)
    {
        int[] newItemID = new int[size];
        int index = 0;
        foreach(int id in target.BlockID)
        {
            if (index < size)
            {
                if (!ignoreOriginValue)
                {
                    newItemID[index] = id;
                } else if(isList)
                {
                    newItemID[index] = index + originIndexSize;
                } else
                {
                    newItemID[index] = 0;
                }
                index++;
            }
        }
        target.BlockID = newItemID;
        return  newItemID;

    }
}

[Serializable]
public class InvType
{
    [SerializeField] private string typeName;
    [SerializeField] private InvMenu[] invMenu;
    #region Getter
    public string TypeName { get { return typeName; } }
    public InvMenu[] invMenus { get { return invMenu; } }
    #endregion
}
[Serializable]
public class InvMenu
{
    [SerializeField] private string menuName;
    [SerializeField] private int[] blockID;
    #region Getter
    public string MenuName { get { return menuName; } }
    public int[] BlockID { get { return blockID; } set { blockID = value; } }
    #endregion

}
