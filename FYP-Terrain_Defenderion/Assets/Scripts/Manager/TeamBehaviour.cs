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
    public int GetTeamID(GameObject gameObject)
    {
        int count = 0;
        foreach(Team team in m_TeamManager)
        {
            if (team.TeamList.Contains(gameObject))
            {
                return count;
            }
            count++;
        }
        return -1;
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
        foreach(Team team in m_TeamManager)
        {
            foreach(GameObject gameObject in team.TeamList)
            {
                Collider collider = gameObject.GetComponent<Collider>();
                if(collider != null)
                {
                    team.TeamColliders.Add(collider);
                }
            }
        }
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
    private List<Collider> teamColliderList = new List<Collider>();
    #region TeamList: Getter & Setter
    public List<GameObject> TeamList { get { return teamList; } }
    public List<Collider> TeamColliders { get { return teamColliderList; } set { teamColliderList = value; } }
    public void AddMember(GameObject target)
    {
        teamList.Add(target);
        teamColliderList.Add(target.GetComponent<Collider>());
    }
    public void RemoveMember(GameObject target)
    {
        teamList.Remove(target);
        teamColliderList.Remove(target.GetComponent<Collider>());
    }
    #endregion

}

