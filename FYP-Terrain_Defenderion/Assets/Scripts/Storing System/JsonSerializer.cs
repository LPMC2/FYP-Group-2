using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
public class JsonSerializer
{
    public void SaveData<T>(T data, string savePath)
    {
            savePath = PathContainsPersistentDataPath(savePath);
        
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + savePath;
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);

        stream.Close();
#if UNITY_EDITOR
        if (File.Exists(savePath))
        {
            Debug.Log("HERE in " + savePath);
        }
#endif
    }
    private static string PathContainsPersistentDataPath(string path)
    {
        if (!path.Contains(Application.persistentDataPath))
        {
            path = Application.persistentDataPath + path;
        }
        return path;
    }
    public static void DeleteFile(string path)
    {
        string filePath = PathContainsPersistentDataPath(path);
        if (File.Exists(filePath))
            File.Delete(filePath);
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
    public static T LoadData<T>(string path) where T : class
    {
        path = PathContainsPersistentDataPath(path);

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            T data = default;
            data = formatter.Deserialize(stream) as T;
            stream.Close();
            return data;
        }
        else
        {
#if UNITY_EDITOR
            Debug.LogWarning("Save file not found in " + path);
#endif
            return null;
        }
    }
    public static T LoadData<T>(TextAsset saveFile) where T : class
    {
        if (saveFile != null)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(saveFile.bytes);
            T data = formatter.Deserialize(stream) as T;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file is null!");
            return null;
        }
    }
}
