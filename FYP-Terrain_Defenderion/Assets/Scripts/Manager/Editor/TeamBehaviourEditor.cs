using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Editor Section
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(TeamBehaviour))]
public class TeamBehaviourEditor : Editor
{
    private GUIStyle labelStyle;
    private void OnEnable()
    {
        labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontStyle = FontStyle.Bold;
        labelStyle.alignment = TextAnchor.MiddleCenter;
    }
    public override void OnInspectorGUI()
    {
        TeamBehaviour teamBehaviour = (TeamBehaviour)target;

        DrawDefaultInspector();
        EditorGUILayout.LabelField("Settings", labelStyle);
        if (GUILayout.Button("Add new Team"))
        {
            teamBehaviour.AddTeam();
        }
        if (GUILayout.Button("Resort Teams"))
        {
            teamBehaviour.SortTeamID();
        }
    }
}
#endif

#endregion