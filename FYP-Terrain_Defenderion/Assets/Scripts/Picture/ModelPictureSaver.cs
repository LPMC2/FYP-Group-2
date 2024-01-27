using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ModelPictureSaver : MonoBehaviour
{
    private static Camera cameraObj;
    private static Renderer modelRenderer;
    private static GameObject modelInstance;
    public static void CaptureAndSaveImage(Camera camera, GameObject modelPrefab, string savePath, string name, bool isDestroy = true, bool lightAllowed = false, float buffer = 0f)
    {
        if (camera == null || modelPrefab == null) return;
        FolderManager.CreateFolder("/StructureData/StructureImg");
        FolderManager.CreateFolder("/StructureData/Temp");
        FolderManager.CreateFolder(savePath);
        int layer = modelPrefab.layer;
        modelPrefab = SetAllChildLayer(modelPrefab, "Capture");
        MeshRenderer meshRenderer = modelPrefab.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            if (!lightAllowed)
            {
                meshRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
        }
        
        // Generate a unique file name
        string fileName = Path.Combine(Application.persistentDataPath + savePath, GenerateFileType(name, "png"));
        //Debug.Log(modelPrefab + " Layer: " + modelPrefab.layer + " Camera: " + camera.cullingMask);
        // Capture the object's image
        var initialCenter = camera.transform.position;
        var initialSize = camera.orthographicSize;
        var (center, size) = CalculateOrthographicSize(modelPrefab, camera, buffer);
        camera.transform.position = center;
        camera.orthographicSize = size;
        CaptureObjectImage(camera, modelPrefab, fileName);
        camera.transform.position = initialCenter;
        camera.orthographicSize = initialSize;
        // Destroy the model instance
        if (isDestroy)
        {
            Object.Destroy(modelPrefab);
            if(modelPrefab != null)
            {
                modelPrefab.SetActive(false);
            }
        }
        modelPrefab = SetAllChildLayer(modelPrefab, LayerMask.LayerToName(layer));
    }
    private static (Vector3 center, float size) CalculateOrthographicSize(GameObject obj, Camera cam, float buffer)
    {
        var bounds = new Bounds();
        var colliders = obj.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            bounds.Encapsulate(col.bounds);
        }
        bounds.Expand(buffer);

        var rotationMatrix = Matrix4x4.TRS(Vector3.zero, cam.transform.rotation, Vector3.one);
        var rotatedCenter = rotationMatrix.MultiplyPoint3x4(bounds.center);
        var rotatedSize = bounds.size;

        var vertical = rotatedSize.y;
        var horizontal = rotatedSize.x * cam.pixelHeight / cam.pixelWidth;

        var size = Mathf.Max(horizontal, vertical) * 0.5f;
        var center = rotatedCenter + cam.transform.forward * -100;

        return (center, size);
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
        if(filePath == null || filePath == "" || cameraObj == null)
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

        //Debug.Log("Image captured and saved as " + filePath);
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
