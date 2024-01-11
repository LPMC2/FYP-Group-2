using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CustomEditor(typeof(DisplayBehaviour))]
public class DisplayBehaviourEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DisplayBehaviour displayBehaviour = (DisplayBehaviour)target;
        if (GUILayout.Button("Preview"))
        {
            displayBehaviour.StartCoroutineInEditor();
        }
    }
}
