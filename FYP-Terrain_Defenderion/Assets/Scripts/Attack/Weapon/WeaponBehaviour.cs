using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class WeaponBehaviour : MonoBehaviour
{
    #region Main Settings
    // Main Settings
    [Tooltip("Time between use")]
    [SerializeField] private Transform m_firePoint;
    [SerializeField] private float m_useCD = 0;
    [SerializeField] private float m_damage;
    [SerializeField] private LayerMask m_affectedLayers;
    [SerializeField] private WeaponFeatures m_features;
    public WeaponFeatures Features { get { return m_features; } }
    [SerializeField] private GameObject m_owner;
    [SerializeField] private float m_activeTime = 1f;
    [SerializeField] private bool m_isActive = true;
    public bool IsActive { get { return m_isActive; } 
        set
        {
            m_isActive = value;
            if(m_isActive)
            {
                useAction.Enable();
            } else
            {
                useAction.Disable();
                if (onUseCoroutine != null)
                {
                    StopCoroutine(onUseCoroutine);
                    onUseCoroutine = null;
                }
            }
        } 
    }

    //Input Settings
    [SerializeField] private InputActionReference m_useWeaponInputActionReference;

    public float useCD { get { return m_useCD; }  }
    public float damage { get { return m_damage; } }
    public LayerMask affectedLayers { get { return m_affectedLayers; } }
    public GameObject owner { get { return m_owner; } }
    public OnUseEvent useEvent;
    public OnHitEvent hitEvent;
    public OnUseCDStart cdStartEvent;
    public OnUseCDEnd cdEndEvent;
    InputAction useAction;
    private bool isPerformaned = false;
    private Coroutine onUseCoroutine;

    //Debug Field
    [SerializeField]private bool isDebug = false;
    private void DebugLog(string text)
    {
        if (isDebug) Debug.Log(text);
    }
    #endregion

    #region Unity Functions
    private void OnValidate()
    {
        if (m_useWeaponInputActionReference == null)
            m_useWeaponInputActionReference = InputActionReference.Create(new PlayerInput().PlayerActions.Right_Click);
    }
    public virtual void Awake()
    {
        if (m_activeTime > 0)
        {
            StartCoroutine(ActiveEnumerator());
        } else
        {
            IsActive = true;
        }
    }
    public virtual void Start()
    {
        if(owner == null && transform.root != transform)
        {
            m_owner = transform.root.gameObject;
        }

    }
    public virtual void Update()
    {
        if (isPerformaned)
        {
            OnUse();
        }
    }
    public virtual void OnEnable()
    {
        m_useWeaponInputActionReference = InputActionReference.Create(new PlayerInput().PlayerActions.Right_Click);
        useAction = m_useWeaponInputActionReference.ToInputAction();
        
        useAction.performed += i => { isPerformaned = true; };
        useAction.canceled += i => { isPerformaned = false; };
        //useAction.canceled += ;
        useEvent += OnUseEvent;
        hitEvent += OnHitEvent;
        cdStartEvent += OnUseCDStartEvent;
        cdEndEvent += OnUseCDEndEvent;
        useAction.Enable();
    }
    public virtual void OnDisable()
    {
        isPerformaned = false;
        useEvent -= OnUseEvent;
        hitEvent -= OnHitEvent;
        cdStartEvent -= OnUseCDStartEvent;
        cdEndEvent -= OnUseCDEndEvent;
        useAction.Disable();
    }
    #endregion

    #region Weapon Features & Variables
    [Flags]
    public enum WeaponFeatures
    {
        DEFAULT = 0,
        AMMO = 1 <<1,
        AIM = 1 <<2,
        ANIMATIONS = 1 <<3,
        SOUNDEFFECTS = 1 << 4,
        RAYCAST = 1 << 5,
        PROJECTILE = 1 << 6

    }
    // Ammo Features Settings
    [SerializeField] private WeaponFeature.AmmoData m_ammoSettings;

    //Raycast Features Settings
    [SerializeField] private WeaponFeature.RayData m_raycastSettings;

    //Projectile Features Settings
    [SerializeField] private WeaponFeature.ProjectileData m_projectileSettings;

    //Animation Settings
    [SerializeField] private WeaponFeature.AnimationData m_animationFeatureSettings;
    #endregion

    #region Events Functions
    public virtual void OnUseEvent()
    {

    }
    public virtual void OnHitEvent(GameObject target)
    {

    }
    public virtual void OnUseCDStartEvent()
    {

    }
    public virtual void OnUseCDEndEvent()
    {

    }


    #endregion

    #region Input Action Functions
    private void OnUse()
    {
        if (onUseCoroutine == null)
        {
            useEvent?.Invoke();
            DebugLog("Performaned!");
            onUseCoroutine = StartCoroutine(OnUseCDEnumerator());
        }
    }
    private IEnumerator OnUseCDEnumerator()
    {
        DebugLog("Start CD!");
        yield return new WaitForSeconds(useCD);
        DebugLog("End CD!");
        StopCoroutine(onUseCoroutine);
        onUseCoroutine = null;
    }
    #endregion

    private IEnumerator ActiveEnumerator()
    {
        m_isActive = false;
        yield return new WaitForSeconds(m_activeTime);
        m_isActive = true;

    }
}

public delegate void OnUseEvent();
public delegate void OnHitEvent(GameObject hitTarget);
public delegate void OnUseCDStart();
public delegate void OnUseCDEnd();