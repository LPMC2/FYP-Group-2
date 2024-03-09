using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class WeaponFeature
{
    [Serializable]
    public class AnimationData
    {
        [SerializeField] private AnimationBehaviour animationSettings;
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
        private int ammoStoringSystemId = -1;
        public int AmmoStoringSystemId { get { return ammoStoringSystemId; } set { ammoStoringSystemId = value; } }
        public InputActionReference ReloadInputActionReference { get { return m_reloadInputActionReference; } set { m_reloadInputActionReference = value; } }
        public int TotalAmmo { get { return m_totalAmmo; } set { m_totalAmmo = value; } }
        public int AmmoCount { get { return m_ammoCount; } set { m_ammoCount = value; } }
        public int RemainAmmo { get { return m_remainAmmo; } set { m_remainAmmo = value; } }
        public float ReloadTime { get { return m_reloadingTime; } }
    }
    [Serializable]
    public class RayData
    {
        [SerializeField] private float m_range = 10f;
        [SerializeField] private int m_bulletCount = 1;
        [SerializeField] private float m_horizontalSpreadAngle = 1f;
        [SerializeField] private float m_verticalSpreadAngle = 1f;
        [SerializeField] private bool m_isPiercing = false;
    }
    [Serializable]
    public class ProjectileData
    {
        [SerializeField] private GameObject m_throwItem;
        [SerializeField] private float m_ProjectileSpeed;
        [SerializeField] private Vector3 m_ProjectileOffset;
        [SerializeField] private bool m_isAreaDamage = false;
        [SerializeField] private float m_AOERadius = 1f;
        [SerializeField] private ProjectileType m_projectileType;
        private GameObject owner;
       
    }
    [Serializable]
    public class AudioData
    {
        [SerializeField] private AudioBehaviour audioSettings;
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
