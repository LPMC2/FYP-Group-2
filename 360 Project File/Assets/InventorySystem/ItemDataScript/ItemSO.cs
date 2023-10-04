using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static QuizSO;

[CreateAssetMenu(fileName = "ItemStorage", menuName ="ScriptableObjects/ItemSO")]
public class ItemSO : ScriptableObject
{
    public Item[] item;
    [System.Serializable]
    public class Item
    {
        public GameObject itemObject;
        public LocalizableString itemName;
        public Sprite itemSprite;
    }
}
