using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(SpawnerBehaviour))]
public class SpawnerBehaviourEditor : Editor
{
    private SerializedProperty maxSpawnTimeProperty;
    private SerializedProperty minSpawnTimeProperty;
    private SerializedProperty spawnRadiusProperty;
    private SerializedProperty spawnCountLimitProperty;
    private SerializedProperty spawnAmountProperty;
    private SerializedProperty stayTimeProperty;
    private SerializedProperty monstersProperty;
        private SerializedProperty targetProperty;
    private bool showPanel = true;
    private GUIStyle leftAlignedLabelStyle;
    private SerializedProperty spawnOnStartProperty;
    private SerializedProperty defaultTeamIdProperty;
    private void OnEnable()
    {
        minSpawnTimeProperty = serializedObject.FindProperty("minSpawnTime");
        maxSpawnTimeProperty = serializedObject.FindProperty("maxSpawnTime");
        spawnRadiusProperty = serializedObject.FindProperty("m_SpawnRadius");
        spawnCountLimitProperty = serializedObject.FindProperty("SpawnCountLimit");
        spawnAmountProperty = serializedObject.FindProperty("SpawnAmout");
        stayTimeProperty = serializedObject.FindProperty("StayTime");
        monstersProperty = serializedObject.FindProperty("monsters");
        targetProperty = serializedObject.FindProperty("TargetObject");
        spawnOnStartProperty = serializedObject.FindProperty("SpawnOnStart");
        defaultTeamIdProperty = serializedObject.FindProperty("defaultTeamId");
    }

    public override void OnInspectorGUI()
    {
       
        serializedObject.Update();
        SpawnerBehaviour spawner = (SpawnerBehaviour)target;
        EditorGUILayout.PropertyField(spawnOnStartProperty);
        EditorGUILayout.PropertyField(defaultTeamIdProperty);
         EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(minSpawnTimeProperty);
        EditorGUILayout.PropertyField(maxSpawnTimeProperty);
         EditorGUILayout.EndHorizontal();
         EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(spawnRadiusProperty);
         EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(spawnCountLimitProperty);
        EditorGUILayout.PropertyField(spawnAmountProperty);
        EditorGUILayout.PropertyField(stayTimeProperty);
        showPanel = EditorGUILayout.Foldout(showPanel, "Spawn Object Settings", EditorStyles.foldoutHeader);
        if (showPanel)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            for (int i = 0; i < monstersProperty.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                SerializedProperty monsterDataProperty = monstersProperty.GetArrayElementAtIndex(i);
                SerializedProperty monsterProperty = monsterDataProperty.FindPropertyRelative("Monster");
                SerializedProperty spawnChanceProperty = monsterDataProperty.FindPropertyRelative("SpawnChance");

                monsterProperty.objectReferenceValue = (GameObject)EditorGUILayout.ObjectField(monsterProperty.objectReferenceValue, typeof(GameObject), true);
                spawnChanceProperty.floatValue = EditorGUILayout.Slider(spawnChanceProperty.floatValue, 0f, 100f);
                EditorGUIUtility.labelWidth -= 45f;
                leftAlignedLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleLeft
                };
                EditorGUILayout.LabelField($"%", leftAlignedLabelStyle, GUILayout.Width(50));
                EditorGUIUtility.labelWidth += 45f;
                EditorGUILayout.BeginHorizontal(EditorStyles.textArea);
                if (GUILayout.Button("+"))
                {
                    spawner.AddMonster(i);
                }
                if (GUILayout.Button("-"))
                {
                    spawner.RemoveMonster(i);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif