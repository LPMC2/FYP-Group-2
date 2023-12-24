using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    [Header("Note: Must contains StrcutureStorage data inside")]
    [SerializeField] private TextAsset[] defaultStructureFiles;
    [SerializeField] private string filePath = "";
    [SerializeField] private int maxStructures = 10;
    [SerializeField] private string[] structurePaths;
    public StructurePooling[] structurePoolings;
    private void Start()
    {
        filePath = Application.persistentDataPath + filePath;
        structurePoolings = new StructurePooling[Directory.GetFiles(filePath).Length + defaultStructureFiles.Length];
        for (int i = 0; i < structurePoolings.Length; i++)
        {
            structurePoolings[i] = new StructurePooling();
        }
        structurePaths = Directory.GetFiles(filePath);
        int count = 0;
        //Default Structures
        foreach(TextAsset textAsset in defaultStructureFiles)
        {
            StructureStorage[] structureStorages = StructureSerializer.LoadObject(textAsset);
            structurePoolings[count].StructureData = structureStorages;
            structurePoolings[count].isDefault = true;
            structurePoolings[count].name = textAsset.name;
            count++;
        }
        //Plauyer Created Structures
        foreach(string structurePath in structurePaths)
        {
            StructureStorage[] structureStorages = StructureSerializer.LoadObject(structurePath, false);
            structurePoolings[count].StructureData = structureStorages;
            structurePoolings[count].name = StructureSerializer.GetFileName(structurePath,1, false);
            count++;
        }

    }
    
    public void LoadAllStructures()
    {
        //Place to rewrite into multiplayer
        GameObject main = new GameObject("Structure Storage");
        foreach(StructurePooling structurePooling in structurePoolings)
        {
            GameObject child = new GameObject(structurePooling.name);
            child.transform.SetParent(main.transform);
            GameObject structure = StructureSerializer.GenerateStructure(structurePooling.StructureData, default, true);

            structure.SetActive(false);
            for (int i = 0; i < maxStructures; i++)
            {
               
                
                    GameObject gameObject1 = Instantiate(structure, Vector3.zero, Quaternion.identity);
                gameObject1.name = structurePooling.name + " - " + i;
                    gameObject1.transform.SetParent(child.transform);
                    structurePooling.structures.Add(gameObject1);
                    gameObject1.SetActive(false);
                
            }
            Destroy(structure);
        }
        //NetworkManager.Singleton.AddNetworkPrefab(gameObject);

    }
}
[Serializable]
public class StructurePooling
{
    public List<GameObject> structures = new List<GameObject>();
    private StructureStorage[] structureData;
    public StructureStorage[] StructureData { get { return structureData; } set { structureData = value; } }
    public string name;
    public int id = -1;
    public bool isDefault = false;
}
