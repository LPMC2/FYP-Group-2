using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AILoadout", menuName = "AI/Create AI Loadout(Structure)")]
public class AILoadoutSO : ScriptableObject
{

    public List<StructureData> structureDatas = new List<StructureData>();

    public void SaveData()
    {
        if (StructureManager.Singleton == null || StructureManager.MainPool == null) {
#if UNITY_EDITOR
            Debug.LogError("Cannot find the MainPool!");
#endif
            return;
        }
        if (structureDatas.Count > 0) { structureDatas.Clear(); }
        int index = 0;
        foreach (Transform child in StructureManager.MainPool.transform)
        {
            foreach (Transform structure in child.transform)
            {
                if (structure.gameObject.activeInHierarchy)
                {
                    structureDatas.Add(new StructureData(index, structure.transform.position, structure.localRotation));

                }
            }
            index++;
        }

    }
    public void LoadData()
    {
        if (StructureManager.Singleton == null || StructureManager.MainPool == null) {
#if UNITY_EDITOR
            Debug.LogError("Cannot find the MainPool!");
#endif
            return;
        }
        foreach (StructureData structure in structureDatas)
        {
            GameObject newStructure = Instantiate(StructureManager.Singleton.structurePoolings[structure.id].structures[0], StructureManager.EnemyStructurePos.transform.localPosition, structure.rotation, StructureManager.EnemyStructurePos.transform);
            newStructure.transform.position = -structure.position;
            newStructure.SetActive(true);
            TeamBehaviour.Singleton.TeamManager[1].AddMember(newStructure);
            SaveSpawner(newStructure);
        }
        StructureManager.EnemyStructurePos.transform.eulerAngles = new Vector3(0f, 0, 0f);
    }
    List<SpawnerBehaviour> spawnerBehaviours = new List<SpawnerBehaviour>();
    private void SaveSpawner(GameObject target)
    {
        foreach (Transform child in target.transform)
        {
            SpawnerBehaviour spawnerBehaviour = child.GetComponent<SpawnerBehaviour>();
            if (spawnerBehaviour != null)
            {
                spawnerBehaviours.Add(spawnerBehaviour);
                spawnerBehaviour.gameObject.GetComponent<Collider>().enabled = true;
                spawnerBehaviour.gameObject.GetComponent<Collider>().isTrigger = false;
            }
        }

    }
    public void LoadSpawner()
    {



        foreach (SpawnerBehaviour spawnerBehaviour in spawnerBehaviours)
        {
            if (spawnerBehaviour)
            {
                TeamBehaviour.Singleton.TeamManager[1].AddMember(spawnerBehaviour.gameObject);
                spawnerBehaviour.StartSpawn(1);
            }
        }
    }

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
