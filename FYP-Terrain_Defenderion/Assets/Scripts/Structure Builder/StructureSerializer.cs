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

    public static void SaveObject(GameObject target, string savePath)
    {
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
                        structureStorages.Add(structureStorage);
                    }
                }
            }
        }
        return structureStorages.ToArray();
    }
    public static int GetFileItemsLength(string path)
    {
        string filePath = "";
        filePath = Application.persistentDataPath + path;
        string[] files = Directory.GetFiles(filePath);
        return files.Length;

    }
    public static StructureStorage[] LoadObject(string savePath)
    {
        string path = Application.persistentDataPath + savePath;
        
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            StructureStorage[] structureStorages = formatter.Deserialize(stream) as StructureStorage[];
            stream.Close();
            Debug.Log("Count: " + structureStorages.Length);
            return structureStorages;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
    public static string GetFileName(string savePath)
    {
        string path = Application.persistentDataPath + savePath;
        if(File.Exists(path))
        {
            string name = Path.GetFileNameWithoutExtension(path);
            name = SetStructureNameFromFile(name);
            //Debug.Log(name);
            return name;
        } else if(savePath.Contains(Application.persistentDataPath))
        {
            string name = Path.GetFileNameWithoutExtension(savePath);
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
                break;
            case StructureType.image:
                directoryPath = Application.persistentDataPath + "/StructureData/StructureImg";
                break;
        }
        string[] files = Directory.GetFiles(directoryPath);
        string[] filesMain = GetFilesInDirectory(files);
        if(id > files.Length)
        {
            return result;
        }
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
    public static string SetStructureNameFromFile(string path)
    {
        path = path.Replace("/StructureData/StructureFile\\", "");
        path = path.Replace(".json", "");
        path = path.Replace("_", " ");
        return path;
    }
    public enum StructureType
    {
        file,
        image
    }
    public static StructureType structureType;
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