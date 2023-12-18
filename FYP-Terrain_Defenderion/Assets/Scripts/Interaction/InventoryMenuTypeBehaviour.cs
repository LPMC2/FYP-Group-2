using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuTypeBehaviour : MonoBehaviour
{
    private InventoryBehaviour inventoryBehaviour;
    private int page = -1;
    Button button;
    public void Initialize(int page, InventoryBehaviour inventoryBehaviour)
    {
        this.inventoryBehaviour = inventoryBehaviour;
        button = GetComponent<Button>();
        this.page = page;
        button.onClick.AddListener(SetBagPage);
    }
    public void SetBagPage()
    {
        inventoryBehaviour.SetMenuPage(page);
    }
}
