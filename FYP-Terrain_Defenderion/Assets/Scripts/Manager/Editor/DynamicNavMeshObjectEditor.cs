using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Unity.AI.Navigation.Samples
{
#if UNITY_EDITOR
    [CustomEditor(typeof(DynamicNavMeshObject))]
    public class DynamicNavMeshObjectEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DynamicNavMeshObject navMeshObject = (DynamicNavMeshObject)target;

            if (GUILayout.Button("Update NavMesh"))
            {
                navMeshObject.UpdateNavMesh();
            }
        }
    }
#endif
}