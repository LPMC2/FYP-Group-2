using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.AI.Navigation.Samples;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StructureManager : MonoBehaviour
{
    private int currentStructure;
    public int CurrentStructure { get { return currentStructure; } set { currentStructure = value; } }
    [Header("Note: Must contains StrcutureStorage data inside")]
    [SerializeField] private TextAsset[] defaultStructureFiles;
    [SerializeField] private InventoryBagSO structureSO;
    public InventoryBagSO StructureSO { get { return structureSO; } }
    [SerializeField] private string structureFilePath = "/StructureData/StructureFile/";
    [SerializeField] private string structureImgPath = "/StructureData/StructureImg/";
    [SerializeField] private string DefaultStructureImgPath = "/StructureData/DefaultStructure/Img/";
    [SerializeField] private int maxStructures = 10;
    [SerializeField] private string[] structurePaths;
    [SerializeField] private Camera captureCamera;
    public StructurePooling[] structurePoolings { get; private set; }
    private GameObject structureStorage;
    private void Start()
    {
        
        structureFilePath = Application.persistentDataPath + structureFilePath;
        FolderManager.CreateFolder(structureFilePath);
        structurePoolings = new StructurePooling[Directory.GetFiles(structureFilePath).Length + defaultStructureFiles.Length];
        for (int i = 0; i < structurePoolings.Length; i++)
        {
            structurePoolings[i] = new StructurePooling();
        }
        structurePaths = Directory.GetFiles(structureFilePath);
        int count = 0;
        //Default Structures
        foreach(TextAsset textAsset in defaultStructureFiles)
        {
            StructureStorage[] structureStorages = StructureSerializer.LoadObject(textAsset);
            structurePoolings[count].StructureData = structureStorages;
            structurePoolings[count].isDefault = true;
            structurePoolings[count].name = StructureSerializer.SetStructureNameFromFile(textAsset.name, false);
            GridManager.SaveStructureImg(textAsset, captureCamera, StructureSerializer.SetStructureNameFromFile(textAsset.name, true), DefaultStructureImgPath);
            structurePoolings[count].structureImage = StructureSerializer.LoadSpriteFromFile(DefaultStructureImgPath + textAsset.name + ".png");
            structurePoolings[count].structureSize = structureStorages[0].gridSize;
            count++;
        }
        int defaultCount = count;
        structureSO.SetItemSize(structureSO.inventoryBag[structureSO.GetIndexFromTypeName("System")].invMenus[0], count, true, true);
        //Player Created Structures
        foreach(string structurePath in structurePaths)
        {
            string name = "";
            StructureStorage[] structureStorages = StructureSerializer.LoadObject(structurePath, false);
            structurePoolings[count].StructureData = structureStorages;
            name = StructureSerializer.GetFileName(structurePath, 1, false);
            structurePoolings[count].name = StructureSerializer.SetStructureNameFromFile(name, false);
            structurePoolings[count].structureImage = StructureSerializer.LoadSpriteFromFile(structureImgPath + name + ".png");
            structurePoolings[count].structureSize = structureStorages[0].gridSize;
            count++;
        }
        structureSO.SetItemSize(structureSO.inventoryBag[structureSO.GetIndexFromTypeName("Custom")].invMenus[0], count-defaultCount, true, true, defaultCount++);
    }
    public void SetDefenseFriendly(GameObject structure, GameObject user)
    {
        int id = TeamBehaviour.Singleton.GetTeamID(user);
        if(id!= -1)
        {
            foreach (Transform child in structure.transform)
            {
                ShooterManager shooterManager = child.GetComponent<ShooterManager>();
                if (shooterManager != null)
                {
                    TeamBehaviour.Singleton.TeamManager[id].AddMember(shooterManager.ShooterObj);
                }
            }
        }
    }
    private List<SpawnerBehaviour> spawnerBehaviours = new List<SpawnerBehaviour>();
    private void StoreSpawners(GameObject structure)
    {
        foreach (Transform child in structure.transform)
        {
            SpawnerBehaviour spawnerBehaviour = child.GetComponent<SpawnerBehaviour>();
            if (spawnerBehaviour != null)
            {
                spawnerBehaviours.Add(spawnerBehaviour);
            }
        }
    }
    public void SetSpawnerTeam()
    {
        foreach (SpawnerBehaviour spawnerBehaviour in spawnerBehaviours)
        {
            if (spawnerBehaviour)
                spawnerBehaviour.StartSpawn(0);
        }
    }
    public virtual GameObject GetStructure(int id, bool isActivation = true)
    {
        if (id < 0 || id > structurePoolings.Length) return null; 
        foreach(GameObject target in structurePoolings[id].structures)
        {
            if(target.activeInHierarchy == false)
            {
                if (isActivation)
                {
                    target.SetActive(true);
                }
                //Debug.Log(target);
                return target;
            }

        }
        return null;
    }
    public virtual void LoadAllStructures()
    {
        if (structureStorage != null) return;
        //Place to rewrite into multiplayer
        GameObject main = new GameObject("Structure Storage");
        foreach(StructurePooling structurePooling in structurePoolings)
        {
            GameObject child = new GameObject(structurePooling.name);
            child.transform.SetParent(main.transform);
            GameObject structure = StructureSerializer.GenerateStructure(structurePooling.StructureData, default, true, true);

            structure.SetActive(false);
            for (int i = 0; i < maxStructures; i++)
            {

                GameObject gameObject1 = Instantiate(structure, Vector3.zero, Quaternion.identity);
                gameObject1.name = structurePooling.name + " - " + i;
                gameObject1.transform.SetParent(child.transform);
                structurePooling.structures.Add(gameObject1);
                Rigidbody rigidbody = gameObject1.GetComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                rigidbody.useGravity = false;
                gameObject1.SetActive(false);
                HealthBehaviour healthBehaviour = gameObject1.GetComponent<HealthBehaviour>();
                if(healthBehaviour != null)
                {
                    healthBehaviour.AddDeathEvents(ScenesManager.Singleton.UpdateNavMeshSurface);
                }
            }
            Destroy(structure);
        }
        //NetworkManager.Singleton.AddNetworkPrefab(gameObject);
        structureStorage = main;
    }
    public virtual void SetDefense()
    {
        foreach (StructurePooling structurePooling in structurePoolings)
        {
            foreach (GameObject structure in structurePooling.structures)
            {
                if(structure.activeInHierarchy)
                {
                    structure.layer = LayerMask.NameToLayer("Defense");
                    DynamicNavMeshObject dynamicNavMeshObject = structure.GetComponent<DynamicNavMeshObject>();
                    HealthBehaviour healthBehaviour = structure.GetComponent<HealthBehaviour>();
                    if(dynamicNavMeshObject != null && healthBehaviour != null)
                    healthBehaviour.AddDeathEvents(dynamicNavMeshObject.UpdateNavMesh);
                    StoreSpawners(structure);
                }
            }
        }
        SetSpawnerTeam();
    }
    public virtual void ResetStorage()
    {
        foreach(StructurePooling structurePooling in structurePoolings)
        {
            int count = 0;
            foreach (GameObject structure in structurePooling.structures)
            {
                structure.transform.position = Vector3.zero;
                structure.transform.rotation = Quaternion.identity;
                structure.SetActive(false);
                if (structurePooling.collider.Length == structurePooling.structures.Count)
                {
                    if(structurePooling.collider[count] != null)
                    {
                        structurePooling.collider[count].enabled = true;
                    }
                }

                count++;
            }
        }
    }
    public virtual void SetStorageCollider()
    {

        foreach (StructurePooling structurePooling in structurePoolings)
        {
            structurePooling.collider = new BoxCollider[maxStructures];
            int count = 0;
            foreach (GameObject structure in structurePooling.structures)
            {
                if (!structure.activeInHierarchy) break;
                BoxCollider boxCollider = structure.GetComponent<BoxCollider>();
                if(boxCollider != null)
                {
                    structurePooling.collider[count] = boxCollider;
                    boxCollider.enabled = false;
                }
                count++;
            }
        }
    }
}
[Serializable]
public class StructurePooling
{
    public List<GameObject> structures = new List<GameObject>();
    private StructureStorage[] structureData;
    public Sprite structureImage;
    public GridSize structureSize;
    public StructureStorage[] StructureData { get { return structureData; } set { structureData = value; } }
    public string name;
    public int id = -1;
    public bool isDefault = false;
    public BoxCollider[] collider;
}
