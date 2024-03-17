using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(InventoryLoadoutSO))]
public class InventoryLoadoutSOEditor : Editor
{
    bool isShow = true;
    bool showItemSOInfo = true;
    public override void OnInspectorGUI()
    {
        InventoryLoadoutSO inventoryLoadoutSO = (InventoryLoadoutSO)target;
        if (inventoryLoadoutSO.ItemScriptableObject == null)
        {
            EditorGUILayout.HelpBox("Warning: Require ItemSO to work!", MessageType.Error);
        }
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        string btnText = isShow ? "Close Info" : "Open Info";
        if (GUILayout.Button(btnText))
        {
            isShow = !isShow;
        }
        if (isShow && inventoryLoadoutSO.ItemScriptableObject != null)
        {


            int i = 0;
            EditorGUILayout.LabelField("Name of Items:");
            foreach (int id in inventoryLoadoutSO.slot)
            {
                i++;
                if (id > -1 && id < inventoryLoadoutSO.ItemScriptableObject.item.Length)
                {
                    EditorGUILayout.LabelField("Slot " + i + ": " + inventoryLoadoutSO.ItemScriptableObject.item[id].itemName);
                } else
                {
                    EditorGUILayout.LabelField("Slot " + i + ": Invaid Item!");
                }

            }

        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
        string btnText2 = showItemSOInfo ? "Close ItemSO Info" : "Open ItemSO Info";
        EditorGUILayout.BeginVertical(GUI.skin.box);
        if (GUILayout.Button(btnText2))
        {
            showItemSOInfo = !showItemSOInfo;
        }
        if (showItemSOInfo && inventoryLoadoutSO.ItemScriptableObject != null)
        {
            for (int i = 0; i < inventoryLoadoutSO.ItemScriptableObject.item.Length; i++)
            {
                EditorGUILayout.LabelField("Slot " + i + ": " + inventoryLoadoutSO.ItemScriptableObject.item[i].itemName);
            }
        }
        EditorGUILayout.EndVertical();
    }
}
#endif