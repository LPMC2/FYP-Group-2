using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class GunController : WeaponBehaviour
{

  
    private Animator anim;


    [Header("Gun Clip Settings")]


    [SerializeField] private int TotalAmmo;

     [SerializeField] private int AmmoCount;

    [SerializeField] private int RemainAmmo;
    [SerializeField] private float ReloadingTime;
    [SerializeField] private UnityEvent ReloadStartFunction;
    [SerializeField] private UnityEvent ReloadEndFunction;

    [Header("Gun Attack Settings")]
    [SerializeField] private GameObject firePoint;
    [SerializeField] private float Range;
    [SerializeField] private int bulletCount = 1;
    [SerializeField] private float horizontalSpreadAngle = 1f;
    [SerializeField] private float verticalSpreadAngle = 1f;
    [SerializeField] private bool isPiercing = false;

    [SerializeField] private UnityEvent<GameObject> HitFunction;
    [SerializeField] private UnityEvent ShootFunction;
    
    [Header("Aim Settings")]
    [SerializeField] private Vector3 AimPosition;
    [SerializeField] private float BulletSpreadMultiplier = 1f;
    [SerializeField] private bool removeAimSymbol = true;
    private Vector3 OriginalPosition;

    [Header("Recoil Settings")]
    [SerializeField]
    private float recoilAngle = 5f;
    // The speed at which the camera rotates back to its original position
    [SerializeField]
    private float recoilSpeed = 10f;
    // The maximum recoil rotation angle
    [SerializeField]
    private float maxRecoilAngle = 20f;
    private bool isRecoiling = false;
    // The current recoil angle
    [SerializeField] private float currentRecoilAngle = 0f;
    private Transform cameraTransform;
    private GameObject cameraObject;
    [Header("Animation Settings")]
    [SerializeField] private AnimationBehaviour animationBehaviour;
    [Header("Other Settings")]
    [SerializeField] private float SpeedMultiplier = 1f;
    [SerializeField] private GameObject hitFX;
    [SerializeField] private bool ignoreAnimation = false;
    [SerializeField] private bool ignoreAudio = false;
    public GameObject Gun;
    [SerializeField] private float SwitchingCD;
    [SerializeField] private GameObject muzzleFlashLight;
    [SerializeField] private ParticleSystem MuzzleFlash;
    [SerializeField] private AudioClip ShootSound;
    [SerializeField] private AudioClip ReloadSound;
    [HideInInspector]public bool isOut = false;
    private GameObject Player;
    private InventoryBehaviour Inventory;
    private float ReloadCD = 0;
    private bool isActive = false;
    private float ActiveTime = 0;
    private float ShootingCD = 0;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private bool isShoot = false;
    private bool isReload = false;
    private bool isAim = false;
    private AudioSource audioSource;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();
    private int ammoStoringSystemId = -1;
    public int AmmoStoringSystemId { get { return ammoStoringSystemId; } set { ammoStoringSystemId = value; } }
    public float GetDamage()
    {
        return damage;
    }
    public int GetTotalAmmo()
    {
        return TotalAmmo;
    }
    public int GetRemainAmmo()
    {
        return RemainAmmo;
    }
    public int GetAmmoCount()
    {
        return AmmoCount;
    }
    public int GetBulletCount()
    {
        return bulletCount;
    }
    public float GetRange()
    {
        return Range;
    }
    public float GetHorAngle()
    {
        return horizontalSpreadAngle;
    }
    public float GetVerAngle()
    {
        return verticalSpreadAngle;
    }
    public float GetSpreadMul()
    {
        return BulletSpreadMultiplier;
    }
    public bool getIsAim()
    {
        return isAim;
    }
    public void SetTotalAmmo(int Ch_TotalAmmo)
    {
        TotalAmmo = Ch_TotalAmmo;
        
    }
    public void SetRemainAmmo(int Ch_RemainAmmo)
    {
        RemainAmmo = Ch_RemainAmmo;
    }
    public void setSpreadAngle(float hor, float ver)
    {
        horizontalSpreadAngle = hor;
        verticalSpreadAngle = ver;
    }
    public void setSpreadMul(float newSpreadMul)
    {
        BulletSpreadMultiplier = newSpreadMul;
    }
    public void setRange(float range)
    {
        Range = range;
    }
  public float GetShootingTime()
    {
        return useCD;
    }
    public bool getActiveState()
    {
        return isActive;
    }
    public void SetActiveState(bool state)
    {
        isActive = state;
        if(state == false)
        {
            ActiveTime = 0f;
        }
    }
    public void setRecoilAngle(float angle)
    {
        recoilAngle = angle;
    }
    public float getRecoilAngle()
    {
        return recoilAngle;
    }

    PlayerLocomotion playerLocomotion;
    AmmoStoringBehaviour ammoStoringBehaviour;
    public override void Start()
    {
        base.Start();
        //cameraTransform = transform.parent;
        Player = gameObject.transform.parent.GetComponent<OriginManager>().OriginGameObject;
        playerLocomotion = Player.GetComponent<PlayerLocomotion>();
        anim = Player.GetComponent<Animator>();
        Inventory = Player.GetComponent<InventoryBehaviour>();
        GetSound();
        if (MuzzleFlash != null)
        {
            MuzzleFlash.Stop();
        }
        OriginalPosition = transform.localPosition;
        StartCoroutine(animationBehaviour.DelayStartAnimationConstant(0.5f,anim, 0, 1f));
        UpdateInv();
        ammoStoringBehaviour = Player.GetComponent<AmmoStoringBehaviour>();
    }
    private void GetSound()
    {
        audioSource = GetComponent<AudioSource>();


        
    }
    public void SetVolume(float Volume)
    {
       
        audioSource.volume *= Volume;

    }
   
    private void Update()
    {
 //if (!isRecoiling)
            //{
            //    currentRecoilAngle = Mathf.Lerp(currentRecoilAngle, 0f, recoilSpeed * Time.deltaTime);
            //}
            //// Rotate the camera back to its original position if the camera is recoiling, but clamp the angle to the current recoil angle
            //else
            //{
            //    currentRecoilAngle = Mathf.Clamp(currentRecoilAngle, 0f, maxRecoilAngle);
            //    currentRecoilAngle = Mathf.Lerp(currentRecoilAngle, 0f, recoilSpeed * Time.deltaTime);
            //    isRecoiling = currentRecoilAngle > 0f; // Set the flag to false if the recoil angle has reached 0
            //}
            //cameraTransform.Rotate(Vector3.left, currentRecoilAngle);
            if (isOut == false && RemainAmmo <= 0 && TotalAmmo <= 0)
            {
                isOut = true;
            }
            else if (isOut == true)
            {
                isOut = false;
            }
            if (isActive == false && SwitchingCD > 0)
            {
                SetActive();
            }
            else if (isActive == false && SwitchingCD == 0)
            {
                isActive = true;
            }
            if (isActive == true)
            {
             if (!playerLocomotion.getIsSprinting())
             {
                ShootDetect();
                Aim();
                }
                ReloadDetect();
                Reload();
            }
        
    }

    private void SetActive()
    {
        if(ActiveTime == 0)
        {
            animationBehaviour.StartAnimationConstant(anim, 1, SwitchingCD);
        }
        ActiveTime += Time.deltaTime;
        if(ActiveTime >= SwitchingCD)
        {
            isActive = true;
        }
        
    }
    private void OnDestroy()
    {
        animationBehaviour.ResetSpeed(anim);
        animationBehaviour.StartAnimationConstant(anim, 4, 1f);
    }
    private void ReloadDetect()
    {
        if (Input.GetKey(KeyCode.R) && ReloadCD <= 0 && RemainAmmo < AmmoCount && isShoot == false && isReload == false)
        {

            isReload = true;
        }
    }
    private void ShootDetect()
    {
        if(isShoot == true)
        {
            fireCoolDown();
        }
        if (Input.GetButton("Fire1") && ReloadCD <= 0 && ShootingCD <= 0 && RemainAmmo > 0 && !isShoot && !isReload)
        {


            //StartCoroutine(playerLocomotion.StartUpdateRotation());
            StartCoroutine(Shoot());
            
            
        }
    }

    private bool hasSound = false;
    private void Reload()

    {

        if ((isReload == true || RemainAmmo <= 0) && TotalAmmo > 0 && isShoot == false)
            
        {
            isReload = true;
            if (audioSource != null && hasSound == false)
            {
                audioSource.clip = ReloadSound;
                audioSource.Play();
                hasSound = true;
            }

            
            if(ReloadCD == 0)
            {
                StartReload();
                ReloadStartFunction.Invoke();
            }
            ReloadCD += Time.deltaTime;

            if (ReloadCD >= ReloadingTime)

            {
                int StoreRemain = RemainAmmo;
                
                if (TotalAmmo >= AmmoCount)
                {
                    RemainAmmo = AmmoCount;
                } else if (TotalAmmo < AmmoCount && (TotalAmmo+StoreRemain <= AmmoCount))
                {
                    RemainAmmo = TotalAmmo+StoreRemain;
                    
                } else
                {
                    RemainAmmo = AmmoCount;
                }
                TotalAmmo -= (AmmoCount - StoreRemain);
                if (TotalAmmo < 0)
                {
                    TotalAmmo = 0;
                }
                UpdateInv();
                ammoStoringBehaviour.StoreAmmo(ammoStoringSystemId, RemainAmmo, TotalAmmo);
                ReloadCD = 0;
                hasSound = false;
                if (audioSource != null)
                {
                    audioSource.clip = null;
                }
                ReloadEndFunction.Invoke();
                isReload = false;
            }

        }

    }

    private void StartRecoil()
    {
        if(!ignoreAnimation)
        animationBehaviour.StartAnimationConstant(anim, 3, 1f);
        
    }
    private void StartReload()
    {

        animationBehaviour.StartAnimationConstant(anim, 2, ReloadingTime);
        


    }

    private void recoilMove()
    {
        return;
        if (cameraTransform != null)
        {
            if (!isRecoiling)
            {
                currentRecoilAngle += recoilAngle;
                currentRecoilAngle = Mathf.Clamp(currentRecoilAngle, 0f, maxRecoilAngle); // Clamp the angle to the maximum value
                isRecoiling = true;
            }
            else // Add the recoil angle to the current recoil angle again if the camera is already recoiling
            {
                currentRecoilAngle += recoilAngle;
                currentRecoilAngle = Mathf.Clamp(currentRecoilAngle, 0f, maxRecoilAngle); // Clamp the angle to the maximum value
            }
        }
    }
   

    private IEnumerator Shoot()
    {
        
        ShootFunction.Invoke();
        isShoot = true;
        RemainAmmo--;
        ammoStoringBehaviour.StoreAmmo(ammoStoringSystemId, RemainAmmo, TotalAmmo);
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(true);
        }
        if (MuzzleFlash != null)
        {
            MuzzleFlash.Play();
        }
        if (audioSource != null)
        {
           if(ignoreAudio == true) 
            {
            if(audioSource.isPlaying == false)
                {
                    audioSource.PlayOneShot(ShootSound, 1);
                }
            } else
            {
                audioSource.PlayOneShot(ShootSound, 1);
            }
            
        }
        recoilMove();
        UpdateInv();

        Vector3 raycastOrigin = firePoint.transform.position + Vector3.up * 0.1f;
        //LayerMask layerMask = LayerMask.GetMask("Default", "Wall", "Enemy","Obstacles");

        for (int i = 0; i < bulletCount; i++)
        {
            float randomHorizontalAngle = Random.Range(-horizontalSpreadAngle / 2, horizontalSpreadAngle / 2);
            float randomVerticalAngle = Random.Range(-verticalSpreadAngle / 2, verticalSpreadAngle / 2);
            Vector3 cameraForward = firePoint.transform.forward;
            Vector3 cameraUp = firePoint.transform.up;
            Vector3 cameraRight = firePoint.transform.right;
            Quaternion horizontalRotation = Quaternion.AngleAxis(randomHorizontalAngle, cameraUp);
            Quaternion verticalRotation = Quaternion.AngleAxis(randomVerticalAngle, cameraRight);
            Quaternion spreadRotation = horizontalRotation * verticalRotation;

            // Calculate the spread direction by rotating the transformed camera's right vector with the spread rotation
            Vector3 spreadDirection = spreadRotation * cameraForward;

            // Draw a debug ray to show the direction of spreadDirection
            Debug.DrawRay(firePoint.transform.position, spreadDirection * Range, Color.blue, 2f);

            RaycastHit hit;

            if (Physics.Raycast(firePoint.transform.position, spreadDirection, out hit, Range, affectedLayers))
            {
                if (!hitEnemies.Contains(hit.collider.gameObject))
                {
                    HitFunction.Invoke(hit.collider.gameObject);
                    IDamageable healthBehaviour = hit.collider.GetComponent<IDamageable>();
                    if (healthBehaviour != null)
                    {
                        healthBehaviour.TakeDamage(damage);
                      
                        if (isPiercing == false)
                        {

                            hitEnemies.Clear();
                            if (bulletCount < 2)
                            {
                                break;
                            }
                        }
                        else if (isPiercing)
                        {

                            continue;
                        }
                    }
                    hitEnemies.Add(hit.collider.gameObject);

                }


            }
        }
        
        
        StartRecoil();
        yield return new WaitForSeconds(useCD*0.1f);
        if (MuzzleFlash != null)
        {
            MuzzleFlash.Stop();
        }
        if (muzzleFlashLight != null)
        {
            muzzleFlashLight.SetActive(false);
        }
        yield return new WaitForSeconds(useCD*0.9f);
        isShoot = false;
            hitEnemies.Clear();

        fireTimer = 0f;
       
        //Update the Inventory UI
       
    }
    private float fireTimer = 0f;
    private void fireCoolDown()
    {
        fireTimer += Time.deltaTime;
        if(fireTimer >= useCD * 2)
        {
            isShoot = false;
            if (MuzzleFlash != null)
            {
                MuzzleFlash.Stop();
            }
            if (muzzleFlashLight != null)
            {
                muzzleFlashLight.SetActive(false);
            }

            fireTimer = 0f;
        }
    }
    private void Aim()
    {

       

        if (isReload == false)
        {
            if (Input.GetButton("Fire2"))
            {
                
                //transform.localPosition = Vector3.Lerp(transform.localPosition, AimPosition, Time.deltaTime * 5f);
              
                if (isAim == false)
                {
                    horizontalSpreadAngle *= BulletSpreadMultiplier;
                    verticalSpreadAngle *= BulletSpreadMultiplier;
                    isAim = true;
                }
            }
            else
            {
                //transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5f);

                if (isAim == true)
                {
                    horizontalSpreadAngle /= BulletSpreadMultiplier;
                    verticalSpreadAngle /= BulletSpreadMultiplier;
                    isAim = false;
                }
            }
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, OriginalPosition, Time.deltaTime * 5f);
            if (isAim == true)
            {
                horizontalSpreadAngle /= BulletSpreadMultiplier;
                verticalSpreadAngle /= BulletSpreadMultiplier;

                isAim = false;
            }
        }
    }
    public void UpdateInv()
    {
        if (Inventory != null)
        {
            Inventory.UpdateSlotDisplay(RemainAmmo + "/" + TotalAmmo);
        }
    }

}