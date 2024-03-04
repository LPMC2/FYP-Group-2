using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AILoadout", menuName = "AI/Create AI Loadout(Structure)")]
public class AILoadoutSO : ScriptableObject
{

    public List<StructureData> structureDatas = new List<StructureData>();
    
    public void SaveData()
    {
        if(StructureManager.Singleton ==null || StructureManager.MainPool == null) {
#if UNITY_EDITOR
            Debug.LogError("Cannot find the structure MainPool!");
            return;
            #endif
        }
            if(structureDatas.Count > 0) { structureDatas.Clear(); }
        int index = 0;
        foreach(Transform child in StructureManager.MainPool.transform)
        {
            foreach(Transform structure in child.transform)
            {
                if(structure.gameObject.activeInHierarchy)
                {
                    structureDatas.Add(new StructureData(index, structure.transform.position, structure.localRotation));

                }
            }
            index++;
        }
        
    }
    public void LoadData()
    {
        if(StructureManager.Singleton == null || StructureManager.MainPool == null) { return; }
        foreach(StructureData structure in structureDatas)
        {
            GameObject newStructure = Instantiate(StructureManager.Singleton.structurePoolings[structure.id].structures[0], StructureManager.EnemyStructurePos.transform.localPosition, structure.rotation, StructureManager.EnemyStructurePos.transform);
            newStructure.transform.position = -structure.position;
            newStructure.SetActive(true);
        }
        StructureManager.EnemyStructurePos.transform.eulerAngles = new Vector3(0f, 180f, 0f);
    }
    [System.Serializable]
    public class StructureData
    {
        public int id;
        public Vector3 position;
        public Quaternion rotation;
        public StructureData (int id, Vector3 pos, Quaternion rot)
        {
            this.id = id;
            position = pos;
            rotation = rot;
        }
    }
}
