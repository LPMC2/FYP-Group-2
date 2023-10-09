using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class FolderManager
{
    public static void CreateFolder(string folderPath)
    {
        //Check if folder exists and create one if not
        if (!folderPath.Contains(Application.persistentDataPath))
        {
            folderPath = Application.persistentDataPath + folderPath;
        }
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log("Folder created at " + folderPath);
        } else
        {
            Debug.Log("Folder already exist at " + folderPath);
        }
    }
}

