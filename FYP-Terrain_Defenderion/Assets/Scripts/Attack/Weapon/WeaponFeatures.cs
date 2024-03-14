using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class WeaponFeature
{
    [Serializable]
    public class AnimationData
    {
        [SerializeField] private AnimationBehaviour animationSettings;
        public AnimationBehaviour AniBehaviour { get { return animationSettings; } }
    }
    [Serializable]
    public class AmmoData 
    {
        // Ammo Features Settings
        [SerializeField] private int m_totalAmmo = 1;

        [SerializeField] private int m_ammoCount = 1;

        [SerializeField] private int m_remainAmmo = 1;
        [SerializeField] private float m_reloadingTime = 1f;
        [SerializeField] private InputActionReference m_reloadInputActionReference;
        [SerializeField] private int m_animationIDOnReload = -1;
        private int ammoStoringSystemId = -1;
        public int AmmoStoringSystemId { get { return ammoStoringSystemId; } set { ammoStoringSystemId = value; } }
        public int AnimationIDReload { get { return m_animationIDOnReload; } }
        public InputActionReference ReloadInputActionReference { get { return m_reloadInputActionReference; } set { m_reloadInputActionReference = value; } }
        public int TotalAmmo { get { return m_totalAmmo; } set { m_totalAmmo = value; } }
        public int AmmoCount { get { return m_ammoCount; } set { m_ammoCount = value; } }
        public int RemainAmmo { get { return m_remainAmmo; } set { m_remainAmmo = value; } }
        public float ReloadTime { get { return m_reloadingTime; } }
        public UnityEvent OnReloadEvent;

    }
    [Serializable]
    public class RayData
    {
        [Header("Main Ray Settings")]
        [SerializeField] private float m_range = 10f;
        [SerializeField] private int m_bulletCount = 1;
        [SerializeField] private float m_horizontalSpreadAngle = 1f;
        [SerializeField] private float m_verticalSpreadAngle = 1f;
        [SerializeField] private bool m_isPiercing = false;
        [SerializeField] private RayType m_type;
        [Header("Visual Settings")]
        [SerializeField] private bool m_isVisible = false;
        [SerializeField] private Material m_material;
        [SerializeField] private AnimationCurve m_laserCurve;
        [SerializeField] private float m_laserClosingTime = 0.5f;
        [Header("Continuous Type Settings")]
        [SerializeField] private float m_fireTime;
        [SerializeField] private float m_preFireCD;
        public bool IsVisible { get { return m_isVisible; } }
        public Material RayMaterial { get { return m_material; } }
        public AnimationCurve RayAnimationCurve { get { return m_laserCurve; } }
        public float RayClosingTime { get { return m_laserClosingTime; } }
        public float FireDuration { get { return m_fireTime; } }
        public float PreFireTime { get { return m_preFireCD; } }
        public RayType rayType { get { return m_type; } }
        public float Range { get { return m_range; } }
        public int BulletCount { get { return m_bulletCount; } }
        public float HSpreadAngle { get { return m_horizontalSpreadAngle; } }
        public float VSpreadAngle { get { return m_verticalSpreadAngle; } }
        public bool IsPiercing { get { return m_isPiercing; } }
        public enum RayType
        {
            ONESHOT,
            CONTINUOUS
        }
    }
    [Serializable]
    public class ProjectileData
    {
        [SerializeField] private GameObject m_throwItem;
        [SerializeField] private float m_ProjectileSpeed;
        [SerializeField] private bool m_isAreaDamage = false;
        [SerializeField] private float m_AOERadius = 1f;
        [SerializeField] private ProjectileType m_projectileType;
        public GameObject ThrowItem { get { return m_throwItem; } }
        public float ProjSpeed { get { return m_ProjectileSpeed; } }
        public bool isAOE { get { return m_isAreaDamage; } }
        public float AOERadius { get { return m_AOERadius; } }
        public ProjectileType Type { get { return m_projectileType; } }

    }
    [Serializable]
    public class AudioData
    {
        [SerializeField] private AudioBehaviour audioSettings;
        public AudioBehaviour AudioBehaviour { get { return audioSettings; } }
    }
    [Serializable]
    public class MeleeData
    {
        [Header("Attack Settings")]
        [SerializeField] private AttackMethod attackMethod;
        [SerializeField] private bool areaAttack = true;
        [SerializeField] private float StartHitTime = 0.5f;
        [Header("Attack - Hitbox Settings")]
        [SerializeField] private float attackDistance = 1f;
        [SerializeField] private float hitboxSizeX = 1f;
        [SerializeField] private float hitboxSizeY = 1f;
        [Header("Misc")]
        [SerializeField] private int AttackSoundID = -1;
        [SerializeField] private int AttackAnimationID = -1;       
        [SerializeField] private float SpeedMultiplier = 1f;
        public AttackMethod AttackMethod { get { return attackMethod; } }
        public bool isAOE { get { return areaAttack; } }
        public float startHitTime { get { return StartHitTime; } }
        public float hitBoxX { get { return hitboxSizeX; } }
        public float hitBoxY { get { return hitboxSizeY; } }
        public float AtkDis { get { return attackDistance; } }
        public int AtkSoundID { get { return AttackSoundID; } }
        public int AtkAniID { get { return AttackAnimationID; } }
        public float SpeedMulti { get { return SpeedMultiplier; } }
    }
    [Flags]
    public enum WeaponFeatures
    {
        DEFAULT = 0,
        AMMO = 1 << 1,
        AIM = 1 << 2,
        ANIMATIONS = 1 << 3,
        SOUNDEFFECTS = 1 << 4,
        RAYCAST = 1 << 5,
        PROJECTILE = 1 << 6,
        ATTACK_COLLISION = 1 << 7

    }
}
