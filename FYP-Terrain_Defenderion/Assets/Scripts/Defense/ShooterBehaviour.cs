using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterBehaviour : MonoBehaviour
{
    [SerializeField] private ShootType bulletType;
    [SerializeField] private GameObject originShooter;
    [SerializeField] private GameObject shootObject;
    [SerializeField] private GameObject HeadObject;
    public GameObject ShootObject { get { return shootObject; } private set { shootObject = value; } }
    [SerializeField] private float preFireCd = 0.1f;
    [SerializeField] private float shootingSpeed = 1f;
    [SerializeField] private float objectSpeed = 1f;
    [SerializeField] private float fireRange = 0.5f;
    [SerializeField] private int shootCount = 1;
    [SerializeField] private float shootSpeedPerCount = 0.1f;
    [SerializeField] private float rotateDuration = 1f;
    [SerializeField] private bool isActive = false;
    [SerializeField] private float range = 10f;
    [SerializeField] private GameObject target;
    [SerializeField] private float damageMultiplier = 1f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField]
    private LayerMask m_HitLayer;
    private bool isRotating = false; // Track if the shooter is currently rotating
    private bool isFiring = false;
    private bool isCD = false; // Track if shooting is in progress
    private bool targetLocked = false;
    private Quaternion targetRotation; // Store the target rotation
    private GameObject rotateObject;
    private void Awake()
    {
        if (originShooter == null)
        {
            originShooter = gameObject;
        }
    }

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
    }

    private bool isCooldown = false; // Track cooldown state
    private float cooldownTimer = 0f; // Cooldown timer
    [SerializeField]
    private float cooldownDuration = 0.5f; // Cooldown duration

    void Update()
    {
        if (isActive && rotateObject != null)
        {
            FindTarget();
            if (target != null && IsTargetInRange(target))
            {
                Quaternion newRotation = Quaternion.LookRotation( target.transform.position - rotateObject.transform.position);
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
                    if (isCooldown)
                    {
                        cooldownTimer += Time.deltaTime;
                        if (cooldownTimer >= cooldownDuration)
                        {
                            isCooldown = false;
                            cooldownTimer = 0f;
                            targetRotation = newRotation;
                            mainTargetRotation = newRotation;
                            isRotating = true;
                            StartCoroutine(RotateToTargetCoroutine());
                        }
                    }
                }
            }
            else
            {
                ResetShooter();
            }
        }

    }
    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetLayer);
        if (colliders.Length > 0)
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
                if (target == null || target.activeInHierarchy == false)
                    target = colliders[0].gameObject;
            }
           
        }
        else
        {
            target = null;
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
          float t = rotationTime / rotateDuration;
              
          Quaternion newRotation = Quaternion.Lerp(startRotation, mainTargetRotation, t);
          rotateObject.transform.rotation = newRotation;
          yield return null;
      }
    
       if (isRotating)
      {
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
            if (targetLocked)
            {
                if (isCD)
                {
                    yield return new WaitForSeconds(preFireCd);
                    isCD = false;
                }
                for (int i = 0; i < shootCount; i++)
                {
                    if (target == null || !IsTargetInRange(target))
                    {
                        isFiring = false;
                        targetLocked = false;
                        yield break; // Stop shooting if target is null or out of range
                    }

                    switch(bulletType)
                    {
                        case ShootType.ForwardObject:
                            FireBullet(ProjectileType.StraightForward);
                            break;
                        case ShootType.MotionObject:
                            FireBullet(ProjectileType.InstantForce);
                            break;
                        case ShootType.Ray:
                            LaserBehaviour laserBehaviour = gameObject.GetComponent<LaserBehaviour>();
                            if(laserBehaviour != null)
                            {
                                laserBehaviour.fire(damageMultiplier);
                            }
                            break;
                    }

                    if (shootCount > 1)
                        yield return new WaitForSeconds(shootSpeedPerCount);
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
        isRotating = false;
    }
    private void FireBullet(ProjectileType projectileType)
    {
        GameObject bullet = Instantiate(shootObject, originShooter.transform.position, Quaternion.identity);
        Projectile projectile = bullet.GetComponent<Projectile>();
        projectile.InitializeProjectile(rotateObject.transform.forward, objectSpeed, damageMultiplier, projectileType);

    }
}
public enum ShootType
{
    ForwardObject,
    MotionObject,
    Ray
}