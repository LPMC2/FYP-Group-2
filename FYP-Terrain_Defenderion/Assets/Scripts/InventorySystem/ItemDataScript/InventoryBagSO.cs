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
    public int[] BlockID { get { return blockID; } }
    #endregion

}
