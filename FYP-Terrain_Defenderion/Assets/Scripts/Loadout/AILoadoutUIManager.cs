using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DanielLochner.Assets.SimpleScrollSnap;

public class AILoadoutUIManager : MonoBehaviour
{
    [SerializeField] private DynamicContent m_ScrollSnapController;
    AILoadoutManager loadoutManager;
    private void Start()
    {
        loadoutManager = AILoadoutManager.Singleton;
        int i = 0;
        foreach(AILoadoutSO loadoutSO in loadoutManager.Data.Datas)
        {
            GameObject panel = m_ScrollSnapController.Add(i);
            LayoutInfoManager layoutInfo = panel.GetComponent<LayoutInfoManager>();
            if (loadoutSO.StructureIcon != null)
            {
                layoutInfo.InfoImg.sprite = loadoutSO.StructureIcon;
            } else
            {
                layoutInfo.InfoImg.sprite = loadoutManager.Data.DefaultLoadoutIcon;
            }
            layoutInfo.Description.text = loadoutSO.Description;
            layoutInfo.Title.text = loadoutSO.name.ToString();
            i++;
        }
    }

}
