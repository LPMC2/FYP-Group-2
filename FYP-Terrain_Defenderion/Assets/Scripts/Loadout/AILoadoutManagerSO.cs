using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AILoadManager_Data", menuName = "Loadout/Create LoadoutManager")]
public class AILoadoutManagerSO : ScriptableObject
{

    [SerializeField] private List<AILoadoutSO> m_loadoutDatas = new List<AILoadoutSO>();
    public void LoadData(int id = -1)
    {
        if (id == -1) return;
        m_loadoutDatas[id].LoadData();
    }
}
