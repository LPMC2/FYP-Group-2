using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILoadoutManager : MonoBehaviour
{
    public static AILoadoutManager Singleton;
    [SerializeField] private int m_loadoutID = -1;
    public int LoadoutID { get { return m_loadoutID; } set { m_loadoutID = value; } }
    [SerializeField] private AILoadoutManagerSO loadoutManagerSO;
    public AILoadoutManagerSO Data { get { return loadoutManagerSO; } }
    private void Awake()
    {
        Singleton = this;    
    }
    public void LoadData(int id)
    {
        Data.LoadData(id);
        SetTurretTeam();
    }

    public void LoadData()
    {
        Data.LoadData(m_loadoutID);
        SetTurretTeam();
    }
    public void SetSpawner()
    {
        Data.SetSpawner(m_loadoutID);
    }
    public void SetTurretTeam()
    {
        HealthBehaviour[] structures =  StructureManager.EnemyStructurePos.GetComponentsInChildren<HealthBehaviour>();
        foreach(HealthBehaviour child in structures)
        {
            TeamBehaviour.Singleton.TeamManager[1].AddMember(child.gameObject);
        }
    }
}
