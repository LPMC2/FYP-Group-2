using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(ConsecutiveActionBehaviour), true)]
public class ConsecutiveActionEditor : Editor
{
    public override void OnInspectorGUI()
    {

        var behaviour = target as IConsecutiveAction;

        if (GUILayout.Button("Test Actions"))
        {
            behaviour.StartActions();
        }
        base.OnInspectorGUI();
    }
}
#endif