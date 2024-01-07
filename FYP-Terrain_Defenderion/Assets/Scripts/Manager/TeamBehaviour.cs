using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamBehaviour : MonoBehaviour
{
    public static TeamBehaviour Singleton;
    [SerializeField] private Team[] m_TeamManager;
    public Team[] TeamManager { get { return m_TeamManager; } }
    public void AddTeam(int count = 1)
    {
        m_TeamManager = arrayBehaviour.AddArray(m_TeamManager, count);
        SortTeamID();
    }
    public void SortTeamID()
    {
        for(int i=0; i< m_TeamManager.Length;i++)
        {
            if(m_TeamManager[i] == null)
            {
                m_TeamManager[i] = new Team();
            }
            m_TeamManager[i].ID = i+1;
        }
    }
    private void Awake()
    {
        Singleton = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[Serializable]
public class Team
{
    [SerializeField] private string name = "";
    public string Name { get { return name; } }
    [SerializeField] private int id = -1;
    public int ID { get { return id; } set { id = value; } }
    [SerializeField] private List<GameObject> teamList = new List<GameObject>();
    #region TeamList: Getter & Setter
    public List<GameObject> TeamList { get { return teamList; } }
    public void AddMember(GameObject target)
    {
        teamList.Add(target);
    }
    public void RemoveMember(GameObject target)
    {
        teamList.Remove(target);
    }
    #endregion

}

