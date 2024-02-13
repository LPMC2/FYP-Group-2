using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionManager : MonoBehaviour
{
    MenuOptionsSelecter optionManager;
    int optionID = -1;
    Button button;

    public void Initialize(MenuOptionsSelecter selecter, int id)
    {
        optionManager = selecter;
        this.optionID = id;
        button = gameObject.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(SelectPage);
        }
    }
    public void SelectPage()
    {
            optionManager.SelectOption(optionID);
    }
}
