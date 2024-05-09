using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStorage", menuName ="ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] item;
   
}
[System.Serializable]
public class Item
{
    public GameObject model;
    public string itemName;
    public bool useItemSprite = false;
    public Sprite itemSprite;
    [TextArea]
    public string description;
    [Header("Capture Settings")]
    public float CustomBuffer = 1f;
    public Vector3 captureAngle = Vector3.zero;
    public Vector3 captureOffset = Vector3.zero;


    public string GetInfo()
    {
        string info = "";
        WeaponBehaviour weaponBehaviour = model.GetComponent<WeaponBehaviour>();
        if (weaponBehaviour != null)
        {
            info = "&" + itemName + "\n\n`Stats:" + "\n~Damage: " + weaponBehaviour.damage + "\n$Use CoolDown: " + weaponBehaviour.useCD + "s"  +(weaponBehaviour.CanSprint ? "\n!Can Sprint" : "")+ "\n`Switch CoolDown: " + weaponBehaviour.ActiveTime + "s";
            if ((weaponBehaviour.Features & WeaponFeature.WeaponFeatures.AMMO) != 0) {
                info += "\nAmmo Count: " + weaponBehaviour.AmmoData.AmmoCount + "\nInitial Ammo: " + weaponBehaviour.AmmoData.TotalAmmo + "\nReload Time: " + weaponBehaviour.AmmoData.ReloadTime + "s";
            }
            if((weaponBehaviour.Features & WeaponFeature.WeaponFeatures.RAYCAST) !=0)
            {
                info += "\n" + (weaponBehaviour.RayData.IsPiercing == true ? "Piercing\n" : "") + "Range: " + weaponBehaviour.RayData.Range;
            }
            if((weaponBehaviour.Features & WeaponFeature.WeaponFeatures.PROJECTILE) != 0)
            {
                info += "\n" + (weaponBehaviour.ProjectileData.isAOE == true ? "Area Damage\n- Radius: " + weaponBehaviour.ProjectileData.AOERadius + "\n" : "");
            }

        } else
        {
            info = "&" + itemName;
        }
        if (description != "")
        {
            info += "\n`" + description;
        }
        return info;
    }
}