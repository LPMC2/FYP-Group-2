using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Inv Loadout", menuName = "Inventory/Create New loadout")]
public class InventoryLoadoutSO : ScriptableObject
{
    [SerializeField] private ItemSO itemSO;
    public ItemSO ItemScriptableObject { get { return itemSO; } }
    public List<int> slot = new List<int>();

}
