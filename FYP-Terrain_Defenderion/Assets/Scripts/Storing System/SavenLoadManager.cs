using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SavenLoadManager<T> : MonoBehaviour where T : class
{
    [SerializeField] private T m_data;
    public T CurrentData { get { return m_data; } }
    [SerializeField] private string m_persistantDataPath = "/";
    [SerializeField] private List<string> m_fileNames = new List<string>();
    public string DataPath { get { return m_persistantDataPath; } set { m_persistantDataPath = value; } }
    public List<string> FileNames { get { return m_fileNames; } set { m_fileNames = value; } }
    public string GetFileDataPath(int id)
    {
        return Application.persistentDataPath + DataPath + FileNames[id] + ".json";
    }
    public virtual void SaveData(int id, T newData)
    {
        if (newData != null)
        {
            m_data = newData;
            JsonSerializer.SaveData(m_data, GetFileDataPath(id));
        }
    }
    public virtual void LoadData(int id)
    {
         m_data = JsonSerializer.LoadData<T>(GetFileDataPath(id));
    }
    public int GetIdFromName(string nameofFile)
    {
        int index = -1;
        foreach(string name in FileNames)
        {
            if(name == nameofFile) { return index; }
            index++;
        }
        return index;
    }
   
}
