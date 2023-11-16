using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapLandmark))]
public class MapLandmarkEditor : Editor
{
    // Disabled: Only for use as subasset
    /*
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        serializedObject.UpdateIfRequiredOrScript();
        var newName = EditorGUILayout.DelayedTextField("Name", target.name, EditorStyles.textField);
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
        if (EditorGUI.EndChangeCheck())
        {
            if (target.name != newName)
            {
                // Rename and mark us as dirty
                target.name = newName;
                EditorUtility.SetDirty(target);

                // Mark main asset as dirty too
                var mainAsset = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(target));
                EditorUtility.SetDirty(mainAsset);

                // Write to disk
                AssetDatabase.SaveAssetIfDirty(mainAsset);
            }
        }
    }
    */
}
