using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
using Unity.AI.Navigation;

public struct StructureTypeFile
{
    public string FileName;
    public string FilePath;
    public Sprite ImageData;
}
public static class StructureSerializer
{
    private static string structurePath = "/StructureData/StructureFile/";
    #region Save Data Functions
    public static void SaveObject(GameObject target, string savePath)
    {
        //Create Folder if needed
        FolderManager.CreateFolder("/StructureData/StructureFile");
        //Save data into a binary form
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + savePath;
        FileStream stream = new FileStream(path, FileMode.Create);
        StructureStorage[] data = SaveStructureData(target);
        formatter.Serialize(stream, data);

        stream.Close();
        if (File.Exists(path))
        {
            Debug.Log("HERE in "  + path);
        }
    }
    public static StructureStorage[] SaveStructureData(GameObject target)
    {
        List<StructureStorage> structureStorages = new List<StructureStorage>();

        foreach (Transform gridCell in target.transform)
        {
            if (gridCell.gameObject.activeInHierarchy)
            {
                GridData gridData = gridCell.GetComponent<GridData>();
                if (gridData != null)
                {
                    Debug.Log("Data");
                        //Debug.Log(gridData.blockId + ": " + gridCell.gameObject);
                        StructureStorage structureStorage = new StructureStorage();

                        structureStorage.structureId = gridData.blockId;
                        structureStorage.cellPos[0] = gridData.cellX;
                        structureStorage.cellPos[2] = gridData.cellY;
                        structureStorage.cellPos[1] = gridData.cellHeight;
                        structureStorage.isAutoRotatable = gridData.isAutoRotatable;
                        structureStorage.Rotation[0] = gridData.Rotation.x;
                        structureStorage.Rotation[1] = gridData.Rotation.y;
                        structureStorage.Rotation[2] = gridData.Rotation.z;
                        structureStorage.Scale[0] = gridData.Scale.x;
                        structureStorage.Scale[1] = gridData.Scale.y;
                        structureStorage.Scale[2] = gridData.Scale.z;
                        structureStorage.tokenCost = gridData.tokenCost;
                        structureStorage.id = gridData.id;
                        structureStorage.isUtility = gridData.isUtility;
                        structureStorage.isDefense = gridData.isDefense;
                        structureStorage.originGameObjectId = gridData.originGameObjectId;
                        //Debug.Log(structureStorage.originGameObjectId +" / " +gridData.originGameObjectId);
                        structureStorage.originInteractType = gridData.originInteractType;
                        structureStorage.gridSize = gridData.gridSize;
                        structureStorages.Add(structureStorage);
                    
                }
            }
        }
        return structureStorages.ToArray();
    }
    #endregion
    public static int GetFileItemsLength(string path)
    {
        FolderManager.CreateFolder(path);
        string filePath = "";
        filePath = Application.persistentDataPath + path;
        string[] files = Directory.GetFiles(filePath);
        return files.Length;

    }
    public static void DeleteFile(string path)
    {
        string filePath = Application.persistentDataPath + path;
        if(File.Exists(filePath))
        File.Delete(filePath);
    }
    #region Load Data Function
    public static StructureStorage[] LoadObject(string savePath, bool includeApplicationDataPath = true)
    {
        string path = "";
        if(includeApplicationDataPath)
        {
            path = Application.persistentDataPath + savePath;
        } else
        {
            path = savePath;
        }
        
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            StructureStorage[] structureStorages = default;
            structureStorages = formatter.Deserialize(stream) as StructureStorage[];
            stream.Close();
            //Debug.Log("Count: " + structureStorages.Length);

            return structureStorages;
        } else
        {
#if UNITY_EDITOR
            Debug.LogWarning("Save file not found in " + path);
#endif
            return null;
        }
    }
    public static StructureStorage[] LoadObject(TextAsset saveFile)
    {
        if (saveFile != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(saveFile.bytes);
            StructureStorage[] structureStorages = formatter.Deserialize(stream) as StructureStorage[];
            stream.Close();
            //Debug.Log("Count: " + structureStorages.Length);
            return structureStorages;
        }
        else
        {
            Debug.LogError("Save file is null!");
            return null;
        }
    }
    //public static Sprite FindImage(string filePath)
    //{
    //    string imagePath = Path.Combine(Application.persistentDataPath, filePath);

    //    if (File.Exists(imagePath))
    //    {
    //        byte[] imageData = File.ReadAllBytes(imagePath);
    //        Texture2D texture = new Texture2D(2, 2);
    //        texture.LoadImage(imageData);

    //        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    //        return sprite;
    //    }
    //    else
    //    {
    //        Debug.LogError("Image file not found at path: " + imagePath);
    //    }
    //    return null;
    //}
    #endregion
    public static string GetFileName(string savePath, int mode = 0, bool includeApplicationDataPath =  true)
    {
        /*  Mode 0 -> Remove _ from file as space
         *  Mode 1 -> Keep all
         * 
         */
        string path = "";
        if (includeApplicationDataPath)
        {
            path = Application.persistentDataPath + savePath;
        }
        else
        {
            path = savePath;
        }
        if (File.Exists(path))
        {
            string name = Path.GetFileNameWithoutExtension(path);
            if(mode == 0)
            name = SetStructureNameFromFile(name);
            //Debug.Log(name);
            return name;
        } else if(savePath.Contains(Application.persistentDataPath))
        {
            string name = Path.GetFileNameWithoutExtension(savePath);
            if(mode == 0)
            name = SetStructureNameFromFile(name);
            return name;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    public static string[] GetFilesInDirectory(string[] files)
    {

        // Remove Application.persistentDataPath from file paths
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = files[i].Replace(Application.persistentDataPath, "");
        }

        return files;
    }
    public static StructureTypeFile SearchStructureFile(int id, StructureType structureType)
    {
        //Debug.Log(id);
        StructureTypeFile result = new StructureTypeFile();
        string directoryPath = "";
       
        switch(structureType)
        {
            case StructureType.file:
                directoryPath = Application.persistentDataPath + "/StructureData/StructureFile";
                FolderManager.CreateFolder(directoryPath);
                break;
            case StructureType.image:
                directoryPath = Application.persistentDataPath + "/StructureData/StructureImg";
                FolderManager.CreateFolder(directoryPath);
                break;
        }
        string[] files = Directory.GetFiles(directoryPath);
        string[] filesMain = GetFilesInDirectory(files);
        if(id > files.Length)
        {
            return result;
        }
        if(files.Length != 0 && filesMain.Length != 0)
        switch(structureType)
        {
            case StructureType.file:
                result.FilePath = filesMain[id];
                result.ImageData = null;
                result.FileName = GetFileName(files[id]);
                break;
            case StructureType.image:
                if (files.Length == filesMain.Length)
                {
                    result.FilePath = filesMain[id];
                    result.ImageData = LoadSpriteFromFile(files[id]);
                    result.FileName = GetFileName(files[id]);
                }
                break;
        }
        return result;
    }
    public static string SetStructureNameFromFile(string path, bool includeUnderline = false)
    {
        path = path.Replace("/StructureData/StructureFile\\", "");
        path = path.Replace(".json", "");
        if(includeUnderline == false)
        path = path.Replace("_", " ");
        return path;
    }
    public static string ReplaceBetweenFileAndImg(string path, int mode = 0, bool haveFileType = true)
    {
        /*  Switch Format:
         *  Mode 0 -> File to Image
         * 
         *  Mode 1 -> Image to File
         */
        switch (mode) 
        {
            case 0:
                path = path.Replace("/StructureData/StructureFile", "/StructureData/StructureImg");
                if(haveFileType)
                    path = path.Replace(".json", ".png");
                break;
            case 1:
                path = path.Replace("/StructureData/StructureImg", "/StructureData/StructureFile");
                if (haveFileType)
                    path = path.Replace(".png", ".json");
                break;
            default:
                Debug.LogWarning("Mode is out of Bounds!");
                break;
        }

        return path;
    }
    public enum StructureType
    {
        file,
        image
    }
    public static StructureType structureType;
    public static void CreateImgFromData(string filePath)
    {
        string structureFilePath = filePath;
        string name = GetFileName(filePath, 1);
        filePath = filePath.Replace(name, "");
        filePath = filePath.Replace(".json", "");
        filePath = ReplaceBetweenFileAndImg(filePath, 0, false);
        //Debug.Log(filePath);
        Camera camera = GameObject.FindGameObjectWithTag("GridCamera").GetComponent<Camera>();
        GameObject gameObject = GenerateStructure(LoadObject(structureFilePath));
        float initialCameraSize = camera.orthographicSize;
        switch (gameObject.GetComponent<GridData>().gridSize)
        {
            case GridSize.Small:
                camera.orthographicSize = 7.69f;
                break;
            case GridSize.Normal:
                camera.orthographicSize = 14.15f;
                break;
            case GridSize.Large:
                camera.orthographicSize = 26.3f;
                break;
        }
        ModelPictureSaver.CaptureAndSaveImage(camera, gameObject, filePath, name, true, false, 4f);
        camera.orthographicSize = initialCameraSize;
    }
    public static GameObject GenerateStructure(StructureStorage[] structureStorage, Vector3 position = default(Vector3), bool combineObjects = false, bool calculateStats = false)
    {
        List<GameObject> utilityList = new List<GameObject>();
        BlockSO blockData = BlockManager.BlockData;
        if (position == default(Vector3))
        {
            position = Vector3.zero; // Set a default value here
        }
        int count = 0;
        int cost = 0;
        float totalHealth = 0f;
        GameObject structure = new GameObject();
        GridData gridData1 = structure.AddComponent<GridData>();
        structure.transform.localPosition = position;
        InteractType interactType = InteractType.none;
        for (int i = 0; i < structureStorage.Length; i++)
        {
            if (structureStorage[i].structureId >= 0)
            {
                GameObject block = MonoBehaviour.Instantiate(blockData.blockData[structureStorage[i].structureId].model, Vector3.zero, Quaternion.identity);
                block.name = blockData.blockData[structureStorage[i].structureId].model.name + ": " + i;
                if (structureStorage[i].isUtility == false && structureStorage[i].isDefense == false)
                {
                    block.transform.SetParent(structure.transform);
                    cost += structureStorage[i].tokenCost;
                    totalHealth += blockData.blockData[structureStorage[i].structureId].maxHealth;
                    if(structureStorage[i].originInteractType != InteractType.none && interactType == InteractType.none)
                    {
                        interactType = structureStorage[i].originInteractType;
                    }
                } else
                {
                    utilityList.Add(block);
                    //Disable collider
                    Collider collider = block.GetComponent<Collider>();
                    if(collider != null)
                    {
                        collider.enabled = false;
                    }
                    //Set Origin Gameobject for shooter
                    ShooterManager shooterManager = block.GetComponent<ShooterManager>();
                    if(shooterManager != null)
                    {
                        shooterManager.shooterBehaviour.Structure = structure;
                    }
                }
                count++;
                block.transform.position = new Vector3((structureStorage[i].cellPos[0]) , (structureStorage[i].cellPos[1]) , (structureStorage[i].cellPos[2]) );
                block.transform.eulerAngles = new Vector3(structureStorage[i].Rotation[0], structureStorage[i].Rotation[1], structureStorage[i].Rotation[2]);
                block.transform.localScale = new Vector3(structureStorage[i].Scale[0] , structureStorage[i].Scale[1] , structureStorage[i].Scale[2] );

                GridData gridData = block.AddComponent<GridData>();
                gridData.SetData(structureStorage[i]);
                gridData1.gridSize = structureStorage[i].gridSize;
                gridData1.originInteractType = interactType;
            }

        }
        if(combineObjects)
        {
            CombineGameObjects(structure);
        }
        if(calculateStats)
        {
            HealthBehaviour healthBehaviour = structure.AddComponent<HealthBehaviour>();
            healthBehaviour.Initialize(totalHealth, true, new Vector3(0f,structure.GetComponent<Renderer>().bounds.size.y * 1.3f ,0f), HealthBarType.WorldSpace, false,false);
            switch(gridData1.gridSize)
            {
                case GridSize.Small:
                    healthBehaviour.SetHPBarSize(new Vector3(5f, 5f, 5f));
                    break;
                case GridSize.Normal:
                    healthBehaviour.SetHPBarSize(new Vector3(7.5f, 7.5f, 7.5f));
                    break;
                case GridSize.Large:
                    healthBehaviour.SetHPBarSize(new Vector3(10f, 10f, 10f));
                    break;
            }
            
            gridData1.tokenCost = cost;
            BoxCollider boxCollider = structure.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            structure.layer = LayerMask.NameToLayer("Grid");
            structure.AddComponent<NavMeshModifier>();
            structure.AddComponent<DynamicNavMeshObject>();
        }
        foreach(GameObject utility in utilityList)
        {
            utility.transform.SetParent(structure.transform);
            utility.GetComponent<Collider>().isTrigger = true;
        }
       
        return structure;
    }
    public static Sprite LoadSpriteFromFile(string filePath)
    {
        filePath = Application.persistentDataPath + filePath;
        //Debug.Log(filePath);
        byte[] imageData = File.ReadAllBytes(filePath);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

        return sprite;
    }
    private static void CombineGameObjects(GameObject parentObject)
    {
        List<GameObject> excludeList = new List<GameObject>();
        foreach (Transform child in parentObject.transform)
        {
            if (child.GetComponent<GridData>().isDefense || child.GetComponent<GridData>().isUtility)
            {
                excludeList.Add(child.gameObject);
                child.SetParent(null);
            }

        }
        MeshCombiner meshCombiner = parentObject.AddComponent<MeshCombiner>();
        meshCombiner.CreateMultiMaterialMesh = true;
        meshCombiner.DestroyCombinedChildren = true;
        meshCombiner.CombineMeshes(true);

        MonoBehaviour.Destroy(meshCombiner);
        MeshCollider meshCollider = parentObject.AddComponent<MeshCollider>();
        MeshFilter meshFilter = parentObject.GetComponent<MeshFilter>();
        meshCollider.sharedMesh = meshFilter.sharedMesh;
        meshCollider.convex = false;
        meshCollider.providesContacts = true;
        foreach (GameObject gameObject in excludeList)
        {
            gameObject.transform.SetParent(parentObject.transform);
        }
    }
    public static void UploadStructure(string path)
    {
        string mainPath = structurePath + Path.GetFileName(path);
        try
        {
            File.Copy(path,Application.persistentDataPath+ mainPath, true);
            Debug.Log("File copied successfully: From - " + path + " to " + Application.persistentDataPath + mainPath);
        }
        catch (IOException e)
        {
            Debug.LogError("Error copying file: " + e.Message);
            return;
        }
        CreateImgFromData(mainPath);
    }
    public static void ExportStructure(string destinationPath, string originPath)
    {
        string name = GetFileName(originPath, 1);
        destinationPath += "/" + name + ".json";

        Debug.Log("File: From - " + originPath + " to " + destinationPath);
        try
        {
            File.Copy(originPath, destinationPath, true);
            Debug.Log("File copied successfully: From - " + originPath + " to " + destinationPath);
        }
        catch (IOException e)
        {
            Debug.LogError("Error copying file: " + e.Message);
            return;
        }
    }
}

[System.Serializable]
public class ObjectData
{
    public string serializedGameObject;
    // Add any other necessary properties
}

public static class BinarySerializationUtility
{
    public static byte[] Serialize(GameObject gameObject)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, gameObject);
            return memoryStream.ToArray();
        }
    }

    public static GameObject Deserialize(byte[] data)
    {
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            var binaryFormatter = new BinaryFormatter();
            return binaryFormatter.Deserialize(memoryStream) as GameObject;
        }
    }
}