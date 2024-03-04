using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(AILoadoutSO))]
public class AILoadoutSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AILoadoutSO data = (AILoadoutSO)target;
        base.OnInspectorGUI();
        GUILayout.Label("Editor Options");
        if(GUILayout.Button("Save Data"))
        {
            data.SaveData();
        }
        if(GUILayout.Button("Test Load Data"))
        {
            data.LoadData();
        }
    }
}
#endif