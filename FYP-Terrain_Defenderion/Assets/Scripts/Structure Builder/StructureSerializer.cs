using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
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
                    if (gridData.blockId >= 0)
                    {
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
                        structureStorages.Add(structureStorage);
                    }
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

    #region Load Data Function
    public static StructureStorage[] LoadObject(string savePath)
    {
        string path = Application.persistentDataPath + savePath;
        
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            StructureStorage[] structureStorages = default;
            structureStorages = formatter.Deserialize(stream) as StructureStorage[];
            stream.Close();
            Debug.Log("Count: " + structureStorages.Length);
            return structureStorages;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    #endregion
    public static string GetFileName(string savePath, int mode = 0)
    {
        /*  Mode 0 -> Remove _ from file as space
         *  Mode 1 -> Keep all
         * 
         */
        string path = Application.persistentDataPath + savePath;
        if(File.Exists(path))
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
                result.FilePath = filesMain[id];
                result.ImageData = LoadSpriteFromFile(files[id]);
                result.FileName = GetFileName(files[id]);
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
        Debug.Log(filePath);
        Camera camera = GameObject.FindGameObjectWithTag("GridCamera").GetComponent<Camera>();
        GameObject gameObject = GenerateStructure(GameObject.Find("Grid").GetComponent<GridManager>(), LoadObject(structureFilePath));
        ModelPictureSaver.CaptureAndSaveImage(camera, gameObject, filePath, name);
    }
    public static GameObject GenerateStructure(GridManager gridManager, StructureStorage[] structureStorage, Vector3 position = default(Vector3))
    {
        if(gridManager == null)
        {
            return null;
        }
        BlockSO blockData = BlockManager.BlockData;
        if (position == default(Vector3))
        {
            position = Vector3.zero; // Set a default value here
        }
        int count = 0;
        GameObject structure = new GameObject();
        structure.transform.localPosition = position;
        for (int i = 0; i < structureStorage.Length; i++)
        {
            GameObject block =  MonoBehaviour.Instantiate(blockData.blockData[structureStorage[i].structureId].blockModel, Vector3.zero, Quaternion.identity);
            block.transform.SetParent(structure.transform);
            count++;
            block.transform.position = new Vector3((structureStorage[i].cellPos[0] - gridManager.numRows / 2) * gridManager.cellSize, (structureStorage[i].cellPos[1] + gridManager.cellSize) * gridManager.cellSize, (structureStorage[i].cellPos[2] - gridManager.numColumns / 2) * gridManager.cellSize);
            block.transform.eulerAngles = new Vector3(structureStorage[i].Rotation[0], structureStorage[i].Rotation[1], structureStorage[i].Rotation[2]);
            block.transform.localScale = new Vector3(structureStorage[i].Scale[0] * gridManager.cellSize, structureStorage[i].Scale[1] * gridManager.cellSize, structureStorage[i].Scale[2] * gridManager.cellSize);

            GridData gridData = block.AddComponent<GridData>();
            gridData.isAutoRotatable = structureStorage[i].isAutoRotatable;
            gridData.cellX = (int)structureStorage[i].cellPos[0];
            gridData.cellY = (int)structureStorage[i].cellPos[2];
            gridData.cellHeight = (int)structureStorage[i].cellPos[1];
            gridData.blockId = structureStorage[i].structureId;
            gridData.Rotation = new Vector3(structureStorage[i].Rotation[0], structureStorage[i].Rotation[1], structureStorage[i].Rotation[2]);
            gridData.Scale = new Vector3(structureStorage[i].Scale[0], structureStorage[i].Scale[1], structureStorage[i].Scale[2]);
            gridData.isUtility = structureStorage[i].isUtility;
            gridData.id = structureStorage[i].id;
            gridData.originGameObjectId = structureStorage[i].originGameObjectId;
            gridData.originInteractType = structureStorage[i].originInteractType;
        }
        Debug.Log("Count: " + count);
        return structure;
    }
    public static Sprite LoadSpriteFromFile(string filePath)
    {
        filePath = Application.persistentDataPath + filePath;

        byte[] imageData = File.ReadAllBytes(filePath);

        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageData);

        Rect rect = new Rect(0, 0, texture.width, texture.height);
        Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);

        return sprite;
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