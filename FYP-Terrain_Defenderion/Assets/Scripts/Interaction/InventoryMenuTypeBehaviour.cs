using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryMenuTypeBehaviour : MonoBehaviour
{
    private InventoryBehaviour inventoryBehaviour;
    private int page = -1;
    Button button;
    InventoryMenuManager menuManager;
    public void Initialize(int page, InventoryBehaviour inventoryBehaviour, InventoryMenuManager inventoryMenuManager)
    {
        this.inventoryBehaviour = inventoryBehaviour;
        button = GetComponent<Button>();
        this.page = page;
        menuManager = inventoryMenuManager;
        button.onClick.AddListener(SetBagPage);
    }

    public void SetBagPage()
    {
        if (page <= -1) return;
        inventoryBehaviour.SetMenuPage(page);
        menuManager.SelectMenuID(page);
    }
}
