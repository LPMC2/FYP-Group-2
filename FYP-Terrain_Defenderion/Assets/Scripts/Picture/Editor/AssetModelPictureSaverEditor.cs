using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(AssetModelPictureSaver))]
public class AssetModelPictureSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AssetModelPictureSaver assetModelPictureSaver = (AssetModelPictureSaver)target;
        GUILayout.Label("Preview");
        GUILayout.Space(20);
        EditorGUI.DrawPreviewTexture(new Rect(0, 120, 200,100), assetModelPictureSaver.texture);
        GUILayout.Space(100);
        //Handles.DrawCamera(previewRect, assetModelPictureSaver.Camera, DrawCameraMode.Normal);
        if (GUILayout.Button("Capture Model"))
        {
            assetModelPictureSaver.Capture();
        }
    }
}
#endif
