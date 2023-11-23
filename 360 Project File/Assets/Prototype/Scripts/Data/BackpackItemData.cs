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
    [Tooltip("Will be called when RunItemIdFunction() is Called & use itemId as parameter")]
    private UnityEvent<int> functionWithItemId;
    [SerializeField]
    [Tooltip("Will be called when RunItemObjectFunction() is Called & use itemObject as parameter")]
    private UnityEvent<GameObject> functionWithItemObj;

    public void RunItemIdFunction()
    {
        functionWithItemId.Invoke(itemId);
    }
    public void RunItemObjectFunction()
    {
        functionWithItemObj.Invoke(itemObject);
    }



}
