using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(InventorySystem))]
public class InventorySystemEditor : Editor
{
    SerializedProperty maxFrontSlotsProperty;

    private void OnEnable()
    {
        maxFrontSlotsProperty = serializedObject.FindProperty("maxFrontSlots");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        InventorySystem inventorySystem = (InventorySystem)target;

        // Exclude script property from the inspector
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(maxFrontSlotsProperty);
        EditorGUI.EndChangeCheck();
        if (inventorySystem != null)
        {
            // Limit the maxFrontSlots value to the count of slot array
            int slotCount = Mathf.Clamp(inventorySystem.slot.Count, 0, 9);

            inventorySystem.SetMaxFrontSlots(Mathf.Clamp(inventorySystem.GetMaxFrontSlots(), 1, slotCount));
        }

        // Exclude maxFrontSlots from the inspector
        SerializedProperty property = serializedObject.GetIterator();
        bool next = property.NextVisible(true);
        while (next)
        {
            if (property.name != "maxFrontSlots" && property.name != "m_Script")
            {
                EditorGUILayout.PropertyField(property, true);
            }
            next = property.NextVisible(false);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif