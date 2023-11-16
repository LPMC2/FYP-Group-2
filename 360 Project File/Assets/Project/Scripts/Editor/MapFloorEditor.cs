using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapFloor))]
public class MapFloorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        using (new LocalizationGroup(target))
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                if (iterator.propertyPath == "m_Script")
                    continue;

                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }
            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();

            // Disabled: Only for use as subasset
            /*
            if (GUILayout.Button("Add Landmark"))
            {
                var instance = target as MapFloor;
                if (instance != null)
                    instance.AddLandmark();
            }
            */
        }
    }
}
