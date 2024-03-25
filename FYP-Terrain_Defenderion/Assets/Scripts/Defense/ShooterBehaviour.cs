using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehaviour : MonoBehaviour
{
    [SerializeField] private ShootType bulletType;
    [SerializeField] private List<GameObject> m_launchPositionObjects =new List<GameObject>();
    [SerializeField] private List<GameObject> m_shootObject = new List<GameObject>();
    [SerializeField] private GameObject HeadObject;
    [SerializeField] private GameObject structure;
    public GameObject Structure { set { structure = value; } }
    public GameObject ShootObject { 
        get 
        {
            if (objectPools != null)
                return arrayBehaviour.GetRandomObjectFromList(objectPools).GetObject(true);
            else
                return null;
        }  
    }
    [SerializeField] private float preFireCd = 0.1f;
    [SerializeField] private float shootingSpeed = 1f;
    [SerializeField] private float objectSpeed = 1f;
    [SerializeField] private float fireRange = 0.5f;
    [SerializeField] private int shootCount = 1;
    [SerializeField] private float shootSpeedPerCount = 0.1f;
    [SerializeField] private Vector3 directionOffset = Vector3.zero;
    [SerializeField] private float rotateDuration = 1f;
    [SerializeField] private Quaternion rotateOffset = Quaternion.identity;
    [SerializeField] private bool isActive = true;
    [SerializeField] private float range = 10f;
    [SerializeField] private GameObject target;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField]
    private LayerMask m_HitLayer;
    [Header("Misc")]
    [SerializeField] private GameObject fireParticleEffect;
    [SerializeField] private string fireSound;
    [SerializeField] private AudioSource audioSource;
    private bool isRotating = false; // Track if the shooter is currently rotating
    private bool isFiring = false;
    private bool isCD = false; // Track if shooting is in progress
    private bool targetLocked = false;
    private Quaternion targetRotation; // Store the target rotation
    private GameObject rotateObject;
    [SerializeField] private bool isDebug = false;
    private void Awake()
    {
        if (m_launchPositionObjects.Count == 0)
        {
            m_launchPositionObjects.Add(gameObject);
        }
    }
    List<ObjectPool> objectPools = new List<ObjectPool>();
    // Start is called before the first frame update
    void Start()
    {
        if(HeadObject == null)
        {
            rotateObject = gameObject;
        } else
        {
            rotateObject = HeadObject;
        }
        if(audioSource == null)
        {
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        foreach(GameObject fireObj in m_shootObject)
        {
            ObjectPool objectPool1 = new ObjectPool();
            objectPool1.Initialize(fireObj,20, gameObject.transform);
            objectPools.Add(objectPool1);
        }
    }

    private bool isCooldown = false; // Track cooldown state
    private float cooldownTimer = 0f; // Cooldown timer
    [SerializeField]
    private float cooldownDuration = 0.5f; // Cooldown duration
    private GameObject currentTarget = null;
    private float cacheExpirationTime = 0.1f; // Time in seconds before cache expires
    private float cacheExpirationTimer; // Timer to track cache expiration
    void Update()
    {
        if (isActive && rotateObject != null)
        {
            if (Time.time >= cacheExpirationTimer)
            {
            
                FindTarget();
                cacheExpirationTimer = Time.time + cacheExpirationTime;
            }
            if (target != null && IsTargetInRange(target))
            {
                Quaternion newRotation = Quaternion.LookRotation( target.transform.position - rotateObject.transform.position) * rotateOffset;
                if (isRotating)
                {
                    if (mainTargetRotation != newRotation)
                    {
                        rotationTime = 0f;
                        startRotation = rotateObject.transform.rotation;
                        mainTargetRotation = newRotation;
                    }
                    if (Quaternion.Angle(targetRotation, newRotation) > fireRange)
                    {
                        targetLocked = true;
                    } else
                    {
                        targetLocked = false;
                    }
                }


                if (!isRotating && Quaternion.Angle(targetRotation, newRotation) > fireRange)
                {
                    if (!isCooldown)
                    {
                        isCooldown = true;
                    }
                   
                }
                if (isCooldown)
                {
                    cooldownTimer += Time.deltaTime;
                    if (cooldownTimer >= cooldownDuration)
                    {
                        isCooldown = false;
                        cooldownTimer = 0f;
                        targetRotation = newRotation ;
                        mainTargetRotation = newRotation ;
                        isRotating = true;
                        StartCoroutine(RotateToTargetCoroutine());
                    }
                }
            }
            else
            {
                ResetShooter();
            }
        }

    }
    private List<GameObject> targets = new List<GameObject>();
    private void GetAllTargets()
    {
        int teamID = TeamBehaviour.Singleton.GetTeamID(gameObject);
        targets.Clear();
        HealthBehaviour[] allTargets = FindObjectsOfType<HealthBehaviour>(true);
        foreach (HealthBehaviour t in allTargets)
        {
            if (targetLayer == (targetLayer | (1 << t.gameObject.layer)))
            {
                targets.Add(t.gameObject);
            }
        }
        targets = TeamBehaviour.Singleton.RemoveFriendlyMembers(teamID, targets);
    }
   
    private GameObject GetNearestTarget()
    {
        Vector3 baseVec3 = transform.position;
        GameObject nearestTarget = null;
        float dis = 0f;
        foreach (GameObject t in targets)
        {
            if (t.activeInHierarchy)
            {
                float newDis = Vector3.Distance(baseVec3, t.transform.position);
                if (dis < newDis && newDis <= range)
                {
                    dis = newDis;
                    nearestTarget = t;
                }
            }
        }
        return nearestTarget;
    }
    private List<Collider> test = new List<Collider>();
    private void FindTarget()
    {
        Collider[] collidersArray = Physics.OverlapSphere(transform.position, range, targetLayer);
        List<Collider> colliders = new List<Collider>(collidersArray);
        arrayBehaviour.DebugList(colliders, "Before:", isDebug);
        test = colliders;
        if (TeamBehaviour.Singleton != null)
        {
            int teamID = TeamBehaviour.Singleton.GetTeamID(gameObject);
            if (teamID != -1)
            {
                colliders.RemoveAll(itemA => TeamBehaviour.Singleton.TeamManager[teamID].TeamList.Contains(itemA.gameObject));
                arrayBehaviour.DebugList(colliders, "After:", isDebug);
            }
        }
        if (colliders.Count > 0)
        {
            float closestDistance = Mathf.Infinity;
            GameObject closestObject = null;

            foreach (Collider collider in colliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = collider.gameObject;

                }
            }

            if (closestObject != null)
            {
                if (target == null || target.activeInHierarchy == false || target.GetComponent<Collider>().enabled == false)
                {
                    if (currentTarget != closestObject)
                    {
                        target =closestObject;
                        currentTarget = closestObject;
                        isCooldown = true;
                        if(bulletType == ShootType.Ray)
                        CloseLaser();
                    }
                   
                }
            }
           
        }
        else
        {
            currentTarget = null;
            target = null;
            if(bulletType == ShootType.Ray)
            CloseLaser();
        }
    }
    private void CloseLaser()
    {
        if (bulletType != ShootType.Ray) return;
        LaserBehaviour laserBehaviour = gameObject.GetComponent<LaserBehaviour>();
        if (laserBehaviour != null)
        {
            laserBehaviour.CloseLaser();
        }
    }
    private bool IsTargetInRange(GameObject target)
    {
        float distance = Vector3.Distance(transform.position, target.transform.position);
        return distance <= range;
    }
    Quaternion startRotation;
    Quaternion mainTargetRotation;
    float rotationTime = 0f;
    private IEnumerator RotateToTargetCoroutine()
    {
        rotationTime = 0f;
       startRotation = rotateObject.transform.rotation;
       while (rotationTime < rotateDuration && isRotating)
       {
            rotationTime += Time.deltaTime;
            if (isDebug) { Debug.Log("Rotating: " + rotationTime + "/" + rotateDuration); }
            float t = rotationTime / rotateDuration;
              
          Quaternion newRotation = Quaternion.Lerp(startRotation, mainTargetRotation, t);
          rotateObject.transform.rotation = newRotation;
          yield return null;
      }
    
       if (isRotating)
      {
            if(isDebug) { Debug.Log("Target Locked and Prepare to fire"); }
          isRotating = false;
            isCD = true;
            targetLocked = true;
          if (!isFiring)
          {
             Shoot();
         }
        }
    }

    private void Shoot()
    {
        StartCoroutine(ShootCoroutine());
        isFiring = true;
    }

    private IEnumerator ShootCoroutine()
    {
        while (true)
        {
            if (targetLocked && target != null)
            {
                if (isCD)
                {
                    yield return new WaitForSeconds(preFireCd);
                    isCD = false;
                }
                Vector3 relativePos = target.transform.position - HeadObject.transform.position + directionOffset;
                if(isDebug) { Debug.DrawRay(HeadObject.transform.position, relativePos, Color.red, 3f); }
                if (Physics.Raycast(HeadObject.transform.position, relativePos, range, m_HitLayer))
                {


                    for (int i = 0; i < shootCount; i++)
                    {
                        if (target == null || !IsTargetInRange(target))
                        {
                            isFiring = false;
                            targetLocked = false;
                            yield break; // Stop shooting if target is null or out of range
                        }

                        switch (bulletType)
                        {
                            case ShootType.ForwardObject:
                                FireBullet(ProjectileType.StraightForward, structure);
                                break;
                            case ShootType.MotionObject:
                                FireBullet(ProjectileType.InstantForce, structure);
                                break;
                            case ShootType.Ray:
                                LaserBehaviour laserBehaviour = gameObject.GetComponent<LaserBehaviour>();
                                if (laserBehaviour != null)
                                {
                                    laserBehaviour.fire(damageMultiplier);
                                }
                                break;
                        }

                        if (shootCount > 1)
                            yield return new WaitForSeconds(shootSpeedPerCount);
                    }
                }
                yield return new WaitForSeconds(shootingSpeed);

            }
            else
            {
                yield return null;
            }
        } 

    }

    private void ResetShooter()
    {
        target = null;
        currentTarget = null;
        isRotating = false;
    }
    private GameObject RandomLaunchPositionObj()
    {
        return arrayBehaviour.GetRandomObjectFromList(m_launchPositionObjects);
    }
    private void FireBullet(ProjectileType projectileType, GameObject owner = null)
    {
        GameObject targetObj = RandomLaunchPositionObj();

        if(fireSound != null)
        {
            AudioManager.Singleton.PlayOneShotSound(fireSound);
        }
       
        Vector3 relativePos = target.transform.position - HeadObject.transform.position + directionOffset;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        RaycastHit hit;


        GameObject bullet = ShootObject;//Instantiate(ShootObject, targetObj.transform.position, Quaternion.identity);
        bullet.transform.position = targetObj.transform.position;
        bullet.transform.rotation = rotation;
        if (fireParticleEffect != null)
        {
            GameObject particle = Instantiate(fireParticleEffect, targetObj.transform.position, rotation*fireParticleEffect.transform.rotation, targetObj.transform);
            particle.transform.localPosition = fireParticleEffect.transform.position;
        }
        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile.InitializeProjectile(rotateObject.transform.forward, objectSpeed, damageMultiplier, projectileType, owner);

    }
}
public enum ShootType
{
    ForwardObject,
    MotionObject,
    Ray
}