using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ModelPictureSaver : MonoBehaviour
{
    private static Camera cameraObj;
    private static Renderer modelRenderer;
    private static GameObject modelInstance;

    public static void CaptureAndSaveImage(Camera camera, GameObject modelPrefab, string savePath, string name, bool isDestroy = true)
    {
        FolderManager.CreateFolder("/StructureData/StructureImg");
        FolderManager.CreateFolder("/StructureData/Temp");
        FolderManager.CreateFolder(savePath);
        modelPrefab = SetAllChildLayer(modelPrefab, "Capture");
        // Generate a unique file name
        string fileName = Path.Combine(Application.persistentDataPath + savePath, GenerateFileType(name, "png"));
        Debug.Log(modelPrefab + " Layer: " + modelPrefab.layer + " Camera: " + camera.cullingMask);
        // Capture the object's image
        CaptureObjectImage(camera, modelPrefab, fileName);

        // Destroy the model instance
        if (isDestroy)
        {
            Object.Destroy(modelPrefab);
            if(modelPrefab != null)
            {
                modelPrefab.SetActive(false);
            }
        }
    }
    public static GameObject SetAllChildLayer(GameObject target, string layerName)
    {
        target.gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in target.transform)
        {
            SetAllChildLayer(child.gameObject, layerName);
        }

        return target;
    }
    //private static bool IsModelFullyRendered(Camera cameraObj, GameObject modelInstance)
    //{
    //    if (modelRenderer == null)
    //    {
    //        modelRenderer = modelInstance.GetComponent<Renderer>();
    //    }

    //    if (modelRenderer != null && modelRenderer.isVisible)
    //    {
    //        // Check if the model's bounds are within the camera's view frustum
    //        Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(cameraObj);
    //        if (!GeometryUtility.TestPlanesAABB(cameraPlanes, modelRenderer.bounds))
    //        {
    //            return false;
    //        }
    //    }

    //    // Check if any child models' bounds are within the camera's view frustum
    //    Renderer[] childRenderers = modelInstance.GetComponentsInChildren<Renderer>();
    //    foreach (Renderer childRenderer in childRenderers)
    //    {
    //        if (childRenderer.isVisible)
    //        {
    //            if (!IsChildModelFullyRendered(childRenderer))
    //            {
    //                return false;
    //            }
    //        }
    //    }

    //    return true;
    //}

    //private static bool IsChildModelFullyRendered(Renderer childRenderer)
    //{
    //    // Check if the child model's bounds are within the camera's view frustum
    //    Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(cameraObj);
    //    return GeometryUtility.TestPlanesAABB(cameraPlanes, childRenderer.bounds);
    //}

    private static void CaptureObjectImage(Camera cameraObj, GameObject modelInstance, string filePath)
    {
        if(filePath == null || filePath == "")
        {
            return;
        } 
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        cameraObj.targetTexture = renderTexture;
        cameraObj.Render();

        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        RenderTexture.active = null;
        cameraObj.targetTexture = null;
        Object.Destroy(renderTexture);
        Object.Destroy(texture);

        Debug.Log("Image captured and saved as " + filePath);
    }
    private static void ImageTransparent(Texture2D image, Camera camera)
    {
        Color bgColor = camera.backgroundColor;
        Color[] pixels = image.GetPixels();

        for(int i=0; i< pixels.Length;i++)
        {
            Color pixel = pixels[i];
            if (pixel == bgColor)
                pixel.a = 0f;
        }
        image.SetPixels(pixels);
        image.Apply();
    }
    private static string GenerateFileType(string name, string type)
    {
        string fileName = name + "." + type;
        return fileName;
    }
}
