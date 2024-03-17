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
    private SerializedProperty m_onUseUnityEventProperty;
    private SerializedProperty m_raycastSettingsProperty;
    private SerializedProperty m_projectileSettingsProperty;
    private SerializedProperty m_animationSettingsProperty;
    private SerializedProperty m_soundEFfectSettingsProperty;
    private SerializedProperty m_onUseAnimationIDProperty;
    private SerializedProperty m_onIdleAnimationIDProperty;
    private SerializedProperty m_resetAniIDProperty;
    private SerializedProperty m_meleeSettingsProperty;
    private SerializedProperty m_useInputProperty;
    private SerializedProperty m_canSprintProperty;
    private SerializedProperty m_OnUseSoundEffectIDroperty;
    private SerializedProperty m_OnActiveSoundEffectIDroperty;
    private SerializedProperty m_ResetSoundEffectIDProperty;
    private SerializedProperty m_onLeaveAnimationIDProperty;
    private SerializedProperty m_onCancelUnityEventProperty;
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
        m_soundEFfectSettingsProperty = serializedObject.FindProperty("m_soundEffectSettings");
        m_onUseUnityEventProperty = serializedObject.FindProperty("onUseUnityEvent");
        m_onUseAnimationIDProperty = serializedObject.FindProperty("m_OnUseAnimationID");
        m_onIdleAnimationIDProperty = serializedObject.FindProperty("m_OnActiveAnimationID");
        m_resetAniIDProperty = serializedObject.FindProperty("m_ResetAnimationID");
        m_meleeSettingsProperty = serializedObject.FindProperty("m_meleeSettings");
        m_useInputProperty = serializedObject.FindProperty("m_useInputAsAction");
        m_canSprintProperty = serializedObject.FindProperty("m_canSprint");
        m_OnUseSoundEffectIDroperty = serializedObject.FindProperty("m_OnUseSoundEffectID");
        m_OnActiveSoundEffectIDroperty = serializedObject.FindProperty("m_OnActiveSoundEffectID");
        m_ResetSoundEffectIDProperty = serializedObject.FindProperty("m_ResetSoundEffectID");
        m_onLeaveAnimationIDProperty = serializedObject.FindProperty("m_OnLeaveAnimationID");
        m_onCancelUnityEventProperty = serializedObject.FindProperty("onCancelUnityEvent");
    }
    WeaponBehaviour weaponBehaviour;
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        weaponBehaviour = (WeaponBehaviour)target;
        serializedObject.Update();

        EditorGUILayout.LabelField("Main - Weapon Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_firePointProperty);
        EditorGUILayout.PropertyField(m_useCDProperty, new GUIContent("Cooldown Time on Use"));
        EditorGUILayout.PropertyField(m_damageProperty);
        EditorGUILayout.PropertyField(m_affectedLayersProperty);
        EditorGUILayout.PropertyField(m_canSprintProperty);
        EditorGUILayout.PropertyField(m_ownerProperty);
        EditorGUILayout.PropertyField(m_activeTimeProperty);
        EditorGUILayout.PropertyField(m_isActiveProperty);
        EditorGUILayout.Space();
        if ((weaponBehaviour.Features & WeaponFeature.WeaponFeatures.ANIMATIONS) != 0)
        {
            EditorGUILayout.BeginVertical(GUI.skin.button);
            EditorGUILayout.LabelField("Animation Settings", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.red } });
            EditorGUILayout.PropertyField(m_onIdleAnimationIDProperty);
            EditorGUILayout.PropertyField(m_onUseAnimationIDProperty);
            EditorGUILayout.PropertyField(m_resetAniIDProperty);
            EditorGUILayout.PropertyField(m_onLeaveAnimationIDProperty);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        if ((weaponBehaviour.Features & WeaponFeature.WeaponFeatures.SOUNDEFFECTS) != 0)
        {
            EditorGUILayout.BeginVertical(GUI.skin.button);
            EditorGUILayout.LabelField("Sound Effect Settings", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });
            EditorGUILayout.PropertyField(m_OnActiveSoundEffectIDroperty);
            EditorGUILayout.PropertyField(m_OnUseSoundEffectIDroperty);
            EditorGUILayout.PropertyField(m_ResetSoundEffectIDProperty);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            
        }
        EditorGUILayout.LabelField("Input Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_useInputProperty);
        if (weaponBehaviour.UseInput)
        {
            EditorGUILayout.PropertyField(m_useWeaponInputActionReferenceProperty);
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Custom Events", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_onUseUnityEventProperty);
        EditorGUILayout.PropertyField(m_onCancelUnityEventProperty);
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Weapon Features", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_featuresProperty);
        EditorGUILayout.Space();
        if (weaponBehaviour.Features != WeaponFeature.WeaponFeatures.DEFAULT)
        {
            EditorGUILayout.BeginVertical(GUI.skin.window);
            EditorGUILayout.Space(-20);
            EditorGUILayout.LabelField("Features List", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.yellow } });

            GenerateFeatureGUI("Ammo Feature", Color.red,ref m_ammoSettingsProperty, WeaponFeature.WeaponFeatures.AMMO);

            GenerateFeatureGUI("Raycast Feature", Color.green, ref m_raycastSettingsProperty, WeaponFeature.WeaponFeatures.RAYCAST);

            GenerateFeatureGUI("Projectile Feature", Color.green, ref m_projectileSettingsProperty, WeaponFeature.WeaponFeatures.PROJECTILE);

            GenerateFeatureGUI("Attack (Melee) Feature", Color.red, ref m_meleeSettingsProperty, WeaponFeature.WeaponFeatures.ATTACK_COLLISION);

            GenerateFeatureGUI("Animation Feature", Color.magenta, ref m_animationSettingsProperty, WeaponFeature.WeaponFeatures.ANIMATIONS);

            GenerateFeatureGUI("Sound Effects Feature", Color.cyan, ref m_soundEFfectSettingsProperty, WeaponFeature.WeaponFeatures.SOUNDEFFECTS);

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_isDebugProperty);
        if(GUILayout.Button("Use Weapon"))
        {
            weaponBehaviour.UseWeapon();
        }

        serializedObject.ApplyModifiedProperties();
    }
    private void GenerateFeatureGUI(string FeatureName, Color color, ref SerializedProperty targetFeatureProperty, WeaponFeature.WeaponFeatures weaponFeature)
    {
        if ((weaponBehaviour.Features & weaponFeature) != 0)
        {

            EditorGUILayout.BeginVertical(GUI.skin.button);
            EditorGUILayout.LabelField(FeatureName, new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = color } });
            EditorGUILayout.LabelField("-------------------------", new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = color } });
            EditorGUILayout.PropertyField(targetFeatureProperty);
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }
    }
}
#endif