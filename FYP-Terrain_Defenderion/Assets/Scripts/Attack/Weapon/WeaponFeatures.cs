using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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

}
