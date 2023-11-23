using UnityEngine;
using UnityEngine.Events;

public class BackpackItemData : MonoBehaviour
{
    public int itemId { get; private set; }

    public GameObject itemObject { get; private set; }
    public void SetItemId(int value)
    {
        itemId = value;
    }
    public void SetItemObject(GameObject itemObj)
    {
        itemObject = itemObj;
    }

    [SerializeField] 
    private ItemIdEvent functionWithItemId;
    [SerializeField]
    private ItemObjectEvent functionWithItemObj;

    public void runItemIdFunction()
    {
        functionWithItemId.Invoke(itemId);
    }
    public void runItemObjectFunction()
    {
        functionWithItemObj.Invoke(itemObject);
    }


    [System.Serializable]
    public class ItemIdEvent : UnityEvent<int> { }
    [System.Serializable]
    public class ItemObjectEvent : UnityEvent<GameObject> { }

}
