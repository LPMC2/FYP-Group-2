using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
public class AssetModelPictureSaver : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    public Camera Camera { get { return m_camera; } }
    [SerializeField] private string m_pathToLocation = "Assets/CapturedPictures/";
    [SerializeField] private float m_buffer = 0f;
    [SerializeField] public Texture2D texture;
    private static int index = 1;
    private static string nameOfFile = "unnamed";

    public void Capture()
    {
        string path = Application.dataPath + m_pathToLocation;
        Debug.Log(path);
        ModelPictureSaver.CaptureAndSaveImage(m_camera, null, path, nameOfFile+index, false, true, m_buffer, false);
        index++;
    }
    private void OnDrawGizmosSelected()
    {
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 8);
        Camera.targetTexture = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();
    }
}
#endif