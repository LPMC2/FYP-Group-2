using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
public class EnemyController : MonoBehaviour
{
    [SerializeField] private float lookRadius = 10f;
    [Header("Attack System")]
    [SerializeField] public AttackType attackType;
    [SerializeField] public ProjectileType projectileType;
    [SerializeField] private float damage;
    [SerializeField] private float preAttackCD;
    [SerializeField] private float attackCD;
    [SerializeField] private UnityEvent customAttackEvent;
    [SerializeField] private GameObject projectileObject;
    private float AttackTime;
    [SerializeField] private float ProjectileSpeed = 5f;
    [SerializeField] private float AttackAngleMultiplier = 1f;
    [SerializeField] private float maxTime;
    [SerializeField] private Vector3 ProjectileOffset;
    [SerializeField] private bool isAreaDamage = false;
    [SerializeField] private float AOERadius = 1f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private int teamID = 0;
    [SerializeField]
    private LayerMask obstacleMask;
    private Animator enemyAnimator;
    private GameObject nearestDestructible;
    [Header("Animation & Sound")]
    [SerializeField] private GameObject AnimateObject;
    [SerializeField] private AudioClip AttackSound;
    [SerializeField] private AudioClip DeathSound;
    private AudioSource audioSource;
    private float closestDistance;
    public enum AttackType
    {
        Melee,
        Ranged,
        Custom
    }
    public ProjectileType CurrentAttackType { get; set; }
    public void changeAttackType(int type)
    {
        switch (type) {
            case 1:
        
                attackType = AttackType.Melee;
                break;
            case 2:
                attackType = AttackType.Ranged;
                break;
        }
    }
    Transform target;
    NavMeshAgent agent;
    private GameObject Siege1;
    private GameObject Siege2;
    HealthBehaviour healthBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        healthBehaviour = GetComponent<HealthBehaviour>();
        audioSource = GetComponent<AudioSource>();
;
        agent = GetComponent<NavMeshAgent>();
        if (AnimateObject != null)
        {
            enemyAnimator = AnimateObject.GetComponent<Animator>();
        }
    }
    public void setAggro(GameObject originTarget)
    {
        target = originTarget.transform;
    }
    public void setViewRadius(float value)
    {
        lookRadius = value;
    }
    public float getViewRadius()
    {
        return lookRadius;
    }
    public void setAttackData(float switchDamage, float switchAttackCD, float switchPreAttackCD)
    {
        damage = switchDamage;
        attackCD = switchAttackCD;
        preAttackCD = switchPreAttackCD;
    }
    public float getDamage()
    {
        return damage;
    }
    public float getAttackCD()
    {
        return attackCD;
    }
    public float getPreAttackCD()
    {
        return preAttackCD;
    }
    public Transform getTarget()
    {
        return target;
    }
    public void playDeathSound()
    {
        if (DeathSound != null && audioSource != null)
        {
            audioSource.clip = DeathSound;
            audioSource.Play();
        }
    }
    GameObject currentTarget;
    private void FindTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, lookRadius, targetLayer);
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
                if (target == null || target.gameObject.activeInHierarchy == false || target.GetComponent<Collider>().enabled == false)
                {
                    if (currentTarget != closestObject)
                    {
                        target = closestObject.transform;
                        currentTarget = closestObject;
                    }

                }
            }

        }
        else
        {
            target = null;
            currentTarget = null;
        }
    }
    // Update is called once per frame
    void Update()
    {

        float distance = Vector3.Distance(target.position, transform.position);
        float destDistance = 0;
        if (nearestDestructible != null)
        {
            destDistance = Vector3.Distance(transform.position, nearestDestructible.transform.position);
        }
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);
            if (distance <= agent.stoppingDistance ||(nearestDestructible != null && (destDistance <= closestDistance)))
            {

                //Attack the target
                FaceTarget();
                StartAttack();
            } else if (distance > agent.stoppingDistance)
            {
                if (AnimateObject != null)
                {
                    if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                    {
                        StartCoroutine(PlayWalkAnimation());
                    }
                }
            }
        }
        else
        {
            if (enemyAnimator != null && attackType != AttackType.Custom)
            {
                enemyAnimator.Play("Idle");
            }
        }
    }
    private IEnumerator PlayWalkAnimation()
    {
        while (true)
        {
            if (enemyAnimator != null)
            {
                // Play the "Walk" animation clip
                float distance = Vector3.Distance(target.position, transform.position);
                if (distance > agent.stoppingDistance)
                {
                    
                    if (healthBehaviour.GetHealth() > 0)
                    {
                        enemyAnimator.Play("Walk");
                    }
                }
            }

            // Wait until the animation clip ends
            yield return new WaitForSeconds(enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
        }
    }
    void FaceTarget()
    {

        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    private void StartAttack()
    {

        AttackTime += Time.deltaTime;

        if (AttackTime >= attackCD)
        {

            StartCoroutine(AttackCoroutine());
            AttackTime = 0;
        }
    }

    private IEnumerator AttackCoroutine()
    {

        if (attackType == AttackType.Melee)
        {
                if (DeathSound != null && audioSource != null)
                {
                    audioSource.clip = AttackSound;
                    audioSource.Play();
                }
            if (enemyAnimator != null)
            {
                // Play the "Walk" animation clip
             
                float speedMultiplier = 1.0f / attackCD;
                enemyAnimator.SetFloat("SpeedMultiplier", speedMultiplier);
                enemyAnimator.Play("AttackMelee");
            }
            yield return new WaitForSeconds(preAttackCD);
            IDamageable damageable = target.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
          

        }
        else if (attackType == AttackType.Ranged)
        {
                if (DeathSound != null && audioSource != null)
                {
                    audioSource.clip = AttackSound;
                    audioSource.Play();
                }
            if (enemyAnimator != null)
            {
                // Play the "Walk" animation clip
                
                float speedMultiplier = 1.0f / attackCD;
                enemyAnimator.SetFloat("SpeedMultiplier", speedMultiplier);
                
                enemyAnimator.Play("AttackRanged");
            }
            yield return new WaitForSeconds(preAttackCD);

                GameObject projectileInstance = Instantiate(projectileObject, transform.position + transform.TransformDirection(ProjectileOffset), Quaternion.identity);
                Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.InitializeProjectile(gameObject.transform.forward,ProjectileSpeed, damage, projectileType);

                }
           
          
        }
        else if (attackType == AttackType.Custom)
        {
            if (DeathSound != null && audioSource != null)
            {
                audioSource.clip = AttackSound;
                audioSource.Play();
            }
            if (enemyAnimator != null)
            {
                // Play the "Walk" animation clip

                float speedMultiplier = 1.0f / attackCD;
                enemyAnimator.SetFloat("SpeedMultiplier", speedMultiplier);

                enemyAnimator.Play("AttackCustom");
            }
            yield return new WaitForSeconds(preAttackCD);
            customAttackEvent.Invoke();
        }
            // Wait for the attack animation to complete before exiting the coroutine
            yield return new WaitForSeconds(attackCD);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}