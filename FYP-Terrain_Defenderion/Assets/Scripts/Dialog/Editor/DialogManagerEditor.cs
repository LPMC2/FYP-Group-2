using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(DialogManager))]
public class DialogManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DialogManager dialogManager = (DialogManager)target;
        if(GUILayout.Button("Test next page"))
        {
            dialogManager.NextPage();
        }
        if(GUILayout.Button("Reset"))
        {
            dialogManager.ResetPage();
        }
    }
}
#endif