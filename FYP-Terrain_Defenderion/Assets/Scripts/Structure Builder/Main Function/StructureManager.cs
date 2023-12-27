using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StructureManager : MonoBehaviour
{
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
    public StructurePooling[] structurePoolings;
    private void Start()
    {
        structureFilePath = Application.persistentDataPath + structureFilePath;
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
    public Sprite structureImage;
    public GridSize structureSize;
    public StructureStorage[] StructureData { get { return structureData; } set { structureData = value; } }
    public string name;
    public int id = -1;
    public bool isDefault = false;
}
