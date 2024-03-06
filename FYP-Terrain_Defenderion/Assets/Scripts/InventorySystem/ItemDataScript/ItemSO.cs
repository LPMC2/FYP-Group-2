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
    [Header("Capture Settings")]
    public float CustomBuffer = 1f;
    public Vector3 captureAngle = Vector3.zero;
    public Vector3 captureOffset = Vector3.zero;
}