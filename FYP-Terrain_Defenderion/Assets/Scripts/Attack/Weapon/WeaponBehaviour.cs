using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Animations.Rigging;

public class WeaponBehaviour : MonoBehaviour
{
    #region Main Settings
    // Main Settings
    [Tooltip("Time between use")]
    [SerializeField] private Transform m_firePoint;
    [SerializeField] private float m_useCD = 0.1f;
    [SerializeField] private float m_damage;
    [SerializeField] private LayerMask m_affectedLayers;
    [SerializeField] private WeaponFeature.WeaponFeatures m_features;
    [SerializeField] private bool m_canSprint = false;
    public WeaponFeature.WeaponFeatures Features { get { return m_features; } }
    [SerializeField] private GameObject m_owner;
    [SerializeField] private float m_activeTime = 1f;
    [SerializeField] private bool m_isActive = true;
    //Animations
    [SerializeField] private int m_OnActiveAnimationID = -1;
    [SerializeField] private List<int> m_OnUseAnimationID = new List<int>();
    [SerializeField] private int m_ResetAnimationID = -1;
    [SerializeField] private int m_OnLeaveAnimationID = -1;
    //Sound Effects
    [SerializeField] private List<int> m_OnUseSoundEffectID = new List<int>();
    [SerializeField] private int m_OnActiveSoundEffectID = -1;
    [SerializeField] private int m_ResetSoundEffectID = -1;
    public bool IsActive { get { return m_isActive; } 
        set
        {
            m_isActive = value;
            if(m_isActive)
            {
                onActiveEvent?.Invoke();
                if(UseInput)
                    useAction.Enable();
            } else
            {

                if (UseInput)
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
    [SerializeField] private bool m_useInputAsAction = true;
    public float ActiveTime { get { return m_activeTime; } }
    public bool UseInput { get { return m_useInputAsAction; } }
    public bool CanSprint { get { return m_canSprint; } }
    public Transform FirePoint { get { return m_firePoint; } }
    public float useCD { get { return m_useCD; }  }
    public float damage { get { return m_damage; } }
    public LayerMask affectedLayers { get { return m_affectedLayers; } }
    public GameObject owner { get { return m_owner; } }
    public OnDestroyEvent onDestroyEvent;
    public OnActiveEvent onActiveEvent;
    [SerializeField] private UnityEvent onUseUnityEvent;
    [SerializeField] private UnityEvent onCancelUnityEvent;
    public OnUseEvent useEvent;
    public OnHitEvent hitEvent;
    public OnUseCDStart cdStartEvent;
    public OnUseCDEnd cdEndEvent;
    public OnUseCancelEvent cancelEvent;
    public bool isCD { get; private set; }
    InputAction useAction;
    private bool isPerformaned = false;
    public Coroutine onUseCoroutine { get; private set; }
    private PlayerManager player;
    private Animator animator;
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
        if (m_useWeaponInputActionReference == null && m_useInputAsAction)
            m_useWeaponInputActionReference = InputActionReference.Create(new PlayerInput().PlayerActions.Right_Click);
    }
    public virtual void Awake()
    {
        AddFeatures();
    }
    public virtual void Start()
    {
        InitialFunction();
    }
    private void ResetEvent()
    {
        useEvent = null;
        hitEvent = null;
        cdStartEvent = null;
        cdEndEvent = null;
        cancelEvent = null;
    }
    private void InitialFunction()
    {
        ResetEvent();
        //Time for enable to use
        if (m_activeTime > 0)
        {
            StartCoroutine(ActiveEnumerator());
        }
        else
        {
            IsActive = true;
        }
        if (owner == null && transform.root != transform)
        {
            m_owner = transform.root.gameObject;
        }
        if (animator == null)
        {
            animator = gameObject.transform.root.GetComponent<Animator>();
        }

        //Animations Feature
        if (owner != null && owner.GetComponent<PlayerManager>() != null)
        {
            player = owner.GetComponent<PlayerManager>();

        }
        if ((Features & WeaponFeature.WeaponFeatures.ANIMATIONS) != 0)
        {
            useEvent += () =>
            {
                PlayAnimation(arrayBehaviour.GetRandomObjectFromList(m_OnUseAnimationID), useCD);
            };
            onDestroyEvent += () =>
            {
                PlayAnimation(m_ResetAnimationID, 1f);
            };
            cancelEvent += () =>
            {
                PlayAnimation(m_OnLeaveAnimationID, 1f);
            };
            GameObjectExtension.DelayEventInvoke(this, () => { PlayAnimation(m_OnActiveAnimationID, m_activeTime); }, 0.01f);
            player.IsRig = m_animationFeatureSettings.AniBehaviour.UseAniRig;
        }

        //Sound Effect Feature
        if ((Features & WeaponFeature.WeaponFeatures.SOUNDEFFECTS) != 0)
        {
            useEvent += () =>
            {
                PlaySoundEffect(arrayBehaviour.GetRandomObjectFromList(m_OnUseSoundEffectID));
            };
            onDestroyEvent += () =>
            {
                PlaySoundEffect(m_ResetSoundEffectID);
            };
            PlaySoundEffect(m_OnActiveSoundEffectID);
        }

        //Sprinting Settings
        GameObjectExtension.DelayEventInvoke(this, () => { player.EnableSprinting = m_canSprint; }, 0.01f);
        onDestroyEvent += () => { player.EnableSprinting = true; };
        useEvent += () => { onUseUnityEvent?.Invoke(); };
        cancelEvent += () => { onCancelUnityEvent?.Invoke(); };
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
        if(m_useWeaponInputActionReference == null && m_useInputAsAction)
            m_useWeaponInputActionReference = InputActionReference.Create(new PlayerInput().PlayerActions.Right_Click);
        if (m_useInputAsAction)
        {
            useAction = m_useWeaponInputActionReference.ToInputAction();
            if (useAction != null)
                useAction.Reset();
            useAction.performed += i => { isPerformaned = true; };
            useAction.canceled += ReleaseUse;
            useAction.Enable();
        }
        //useAction.canceled += ;
        useEvent += OnUseEvent;
        hitEvent += OnHitEvent;
        cdStartEvent += OnUseCDStartEvent;
        cdEndEvent += OnUseCDEndEvent;
    }
    private void ReleaseUse(InputAction.CallbackContext callbackContext)
    {
        isPerformaned = false; cancelEvent?.Invoke();
    }
    public virtual void OnDisable()
    {
        isPerformaned = false;
        useEvent -= OnUseEvent;
        hitEvent -= OnHitEvent;
        cdStartEvent -= OnUseCDStartEvent;
        cdEndEvent -= OnUseCDEndEvent;
        useAction.canceled -= ReleaseUse;
        if (m_useInputAsAction)
            useAction.Disable();
    }
    private void OnDestroy()
    {
        DebugLog("Destroyed!");
        onDestroyEvent?.Invoke();
        if (useAction != null)
        {
            useAction.canceled -= ReleaseUse;
        }
    }
    #endregion

    #region Weapon Features & Variables

    // Ammo Features Settings
    [SerializeField] private WeaponFeature.AmmoData m_ammoSettings;
    public WeaponFeature.AmmoData AmmoData { get { return m_ammoSettings; } }

    //Raycast Features Settings
    [SerializeField] private WeaponFeature.RayData m_raycastSettings;
    public WeaponFeature.RayData RayData { get { return m_raycastSettings; } }

    //Projectile Features Settings
    [SerializeField] private WeaponFeature.ProjectileData m_projectileSettings;
    public WeaponFeature.ProjectileData ProjectileData { get { return m_projectileSettings; } }
    //Animation Settings
    [SerializeField] private WeaponFeature.AnimationData m_animationFeatureSettings;
    public WeaponFeature.AnimationData AnimationData { get { return m_animationFeatureSettings; } }
    //Audio Features Settings
    [SerializeField] private WeaponFeature.AudioData m_soundEffectSettings;
    public WeaponFeature.AudioData AudioData { get { return m_soundEffectSettings; } }
    //Melee Settings
    [SerializeField] private WeaponFeature.MeleeData m_meleeSettings;
    public WeaponFeature.MeleeData MeleeData { get { return m_meleeSettings; } }

    private void AddFeatures()
    {
        if((m_features & WeaponFeature.WeaponFeatures.AMMO)!= 0)
        {
            AmmoBehaviour ammoBehaviour = gameObject.AddComponent<AmmoBehaviour>();
            ammoBehaviour.SetBehaviour(this);
            useEvent += ammoBehaviour.UseAmmo;
        }
        if ((m_features & WeaponFeature.WeaponFeatures.PROJECTILE) != 0)
        {
            ItemThrowerBehaviour itemThrowerBehaviour = gameObject.AddComponent<ItemThrowerBehaviour>();
            itemThrowerBehaviour.WBehaviour = this;
            useEvent += itemThrowerBehaviour.Shoot;
        }
        if ((m_features & WeaponFeature.WeaponFeatures.RAYCAST) != 0) { 
            RaycastBehaviour rayBehaviour = gameObject.AddComponent<RaycastBehaviour>();
            rayBehaviour.WBehaviour = this;
            useEvent += rayBehaviour.StartFireRay;
        }
        if ((m_features & WeaponFeature.WeaponFeatures.AIM) != 0) { }
        if((m_features & WeaponFeature.WeaponFeatures.ANIMATIONS) != 0)
        {
            
        }
        if((m_features & WeaponFeature.WeaponFeatures.ATTACK_COLLISION) != 0)
        {
            MeleeController meleeController = gameObject.AddComponent<MeleeController>();
            meleeController.weaponBehaviour = this;
            useEvent += meleeController.AttackWeapon;
        }
    }
    #endregion

    #region Events Functions
    //For Method Overriding
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
    public void UseWeapon()
    {
        OnUse();
    }
    #endregion

    #region Enumerators
    private IEnumerator ActiveEnumerator()
    {
        IsActive = false;
        yield return new WaitForSeconds(m_activeTime);
        IsActive = true;

    }
    private IEnumerator OnUseCDEnumerator()
    {
        isCD = true;
        DebugLog("Start CD!");
        yield return new WaitForSeconds(useCD);
        DebugLog("End CD!");
        StopCoroutine(onUseCoroutine);
        onUseCoroutine = null;
        isCD = false;
    }
    private void StopUseCDEnumerator()
    {
        StopCoroutine(onUseCoroutine);
        onUseCoroutine = null;
    }

    #endregion

    public void HitTakeDamage(GameObject hit)
    {

        IDamageable healthBehaviour = hit.GetComponent<IDamageable>();
        if (healthBehaviour != null)
        {
            healthBehaviour.TakeDamage(damage);
        }
    }

    #region Animation & Sound Effects Player
    public void PlayAnimation(int id, float speed)
    {
        if((Features & WeaponFeature.WeaponFeatures.ANIMATIONS) != 0)
            m_animationFeatureSettings.AniBehaviour.StartAnimationConstant(animator, id, speed);
    }
    public void PlaySoundEffect(int id)
    {
        if ((Features & WeaponFeature.WeaponFeatures.SOUNDEFFECTS) != 0)
            m_soundEffectSettings.AudioBehaviour.PlayAudio(id);
    }
    #endregion
}
public delegate void OnUseCancelEvent();
public delegate void OnUseEvent();
public delegate void OnHitEvent(GameObject hitTarget);
public delegate void OnUseCDStart();
public delegate void OnUseCDEnd();
public delegate void OnActiveEvent();
public delegate void OnDestroyEvent();