using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(WeaponBehaviour))]
public class WeaponBehaviourEditor : Editor
{
    private SerializedProperty m_firePointProperty;
    private SerializedProperty m_useCDProperty;
    private SerializedProperty m_damageProperty;
    private SerializedProperty m_affectedLayersProperty;
    private SerializedProperty m_featuresProperty;
    private SerializedProperty m_ownerProperty;
    private SerializedProperty m_activeTimeProperty;
    private SerializedProperty m_isActiveProperty;
    private SerializedProperty m_isDebugProperty;
    private SerializedProperty m_useWeaponInputActionReferenceProperty;
    private SerializedProperty m_ammoSettingsProperty;

    private SerializedProperty m_raycastSettingsProperty;
    private SerializedProperty m_projectileSettingsProperty;
    private SerializedProperty m_animationSettingsProperty;
    private SerializedProperty m_soundEFfectSettingsProperty;


    private void OnEnable()
    {
        m_firePointProperty = serializedObject.FindProperty("m_firePoint");
        m_useCDProperty = serializedObject.FindProperty("m_useCD");
        m_damageProperty = serializedObject.FindProperty("m_damage");
        m_affectedLayersProperty = serializedObject.FindProperty("m_affectedLayers");
        m_featuresProperty = serializedObject.FindProperty("m_features");
        m_ownerProperty = serializedObject.FindProperty("m_owner");
        m_activeTimeProperty = serializedObject.FindProperty("m_activeTime");
        m_isActiveProperty = serializedObject.FindProperty("m_isActive");
        m_isDebugProperty = serializedObject.FindProperty("isDebug");
        m_useWeaponInputActionReferenceProperty = serializedObject.FindProperty("m_useWeaponInputActionReference");
        m_ammoSettingsProperty = serializedObject.FindProperty("m_ammoSettings");
        m_raycastSettingsProperty = serializedObject.FindProperty("m_raycastSettings");
        m_projectileSettingsProperty = serializedObject.FindProperty("m_projectileSettings");
        m_animationSettingsProperty = serializedObject.FindProperty("m_animationFeatureSettings");
    }
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        WeaponBehaviour weaponBehaviour = (WeaponBehaviour)target;
        serializedObject.Update();

        EditorGUILayout.LabelField("Main - Weapon Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_firePointProperty);
        EditorGUILayout.PropertyField(m_useCDProperty, new GUIContent("Cooldown Time on Use"));
        EditorGUILayout.PropertyField(m_damageProperty);
        EditorGUILayout.PropertyField(m_affectedLayersProperty);
        EditorGUILayout.PropertyField(m_ownerProperty);
        EditorGUILayout.PropertyField(m_activeTimeProperty);
        EditorGUILayout.PropertyField(m_isActiveProperty);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Input Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_useWeaponInputActionReferenceProperty);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weapon Features", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_featuresProperty);
        EditorGUILayout.Space();
        if (weaponBehaviour.Features != WeaponBehaviour.WeaponFeatures.DEFAULT)
        {
            EditorGUILayout.BeginVertical(GUI.skin.window);
            EditorGUILayout.Space(-20);
            EditorGUILayout.LabelField("Features List", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });
            if ((weaponBehaviour.Features & WeaponBehaviour.WeaponFeatures.AMMO) != 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.button);

                EditorGUILayout.LabelField("Ammo Feature", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.red } });
                EditorGUILayout.LabelField("-------------------------", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.red } });
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_ammoSettingsProperty);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            if ((weaponBehaviour.Features & WeaponBehaviour.WeaponFeatures.RAYCAST) != 0)
            {

                EditorGUILayout.BeginVertical(GUI.skin.button);

                EditorGUILayout.LabelField("Raycast Feature", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.green } });
                EditorGUILayout.LabelField("-------------------------", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.green } });
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_raycastSettingsProperty);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }
            if ((weaponBehaviour.Features & WeaponBehaviour.WeaponFeatures.PROJECTILE) != 0)
            {

                EditorGUILayout.BeginVertical(GUI.skin.button);
                EditorGUILayout.LabelField("Projectile Feature", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.green } });
                EditorGUILayout.LabelField("-------------------------", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.green } });
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_projectileSettingsProperty);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }
            if ((weaponBehaviour.Features & WeaponBehaviour.WeaponFeatures.ANIMATIONS) != 0)
            {

                EditorGUILayout.BeginVertical(GUI.skin.button);
                EditorGUILayout.LabelField("Animation Feature", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.magenta } });
                EditorGUILayout.LabelField("-------------------------", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.magenta } });
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(m_animationSettingsProperty);
                EditorGUILayout.Space();
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_isDebugProperty);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif