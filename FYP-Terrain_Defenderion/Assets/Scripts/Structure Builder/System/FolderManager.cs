using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class FolderManager
{
    public static void CreateFolder(string folderPath, bool includePersistancePath = true)
    {
        //Check if folder exists and create one if not
        if (!folderPath.Contains(Application.persistentDataPath) && includePersistancePath)
        {
            folderPath = Application.persistentDataPath + folderPath;
        }
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
#if UNITY_EDITOR
            Debug.Log("Folder created at " + folderPath);
#endif
        } else
        {
#if UNITY_EDITOR
            Debug.Log("Folder already exist at " + folderPath);
#endif
        }
    }
#if UNITY_EDITOR
    public static void CreateAssetFolder(string name)
    {
        if (!AssetDatabase.IsValidFolder("Assets/" + name)) {
            AssetDatabase.CreateFolder("Assets", name);
        }
    }
#endif
}

