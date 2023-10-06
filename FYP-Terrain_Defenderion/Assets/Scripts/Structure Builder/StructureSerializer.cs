using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

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
            Debug.Log(name);
            return name;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
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