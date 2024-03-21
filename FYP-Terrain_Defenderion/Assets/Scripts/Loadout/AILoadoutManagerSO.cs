using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AILoadManager_Data", menuName = "AI/Create LoadoutManager")]
public class AILoadoutManagerSO : ScriptableObject
{
    [SerializeField] private Sprite m_defaultLoadoutIcon;
    [SerializeField] private List<AILoadoutSO> m_loadoutDatas = new List<AILoadoutSO>();
    public List<AILoadoutSO> Datas { get { return m_loadoutDatas; } }
    public Sprite DefaultLoadoutIcon { get { return m_defaultLoadoutIcon; } }
    public void LoadData(int id = -1)
    {
        if (id == -1) return;
        m_loadoutDatas[id].LoadData();
    }
    public void SetSpawner(int id)
    {
        m_loadoutDatas[id].LoadSpawner();
    }
}
