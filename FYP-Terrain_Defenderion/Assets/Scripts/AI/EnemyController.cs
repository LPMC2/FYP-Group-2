using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System.Linq;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private AIState enemyState;
    [Header("Team Settings")]
    [SerializeField] private int m_DefaultTeamId = -1;
    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float fleeSpeed = 10f;
    [Header("Searching & Patrol Settings")]
    [SerializeField] private float lookRadius = 10f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField, Layer] private int preferredTargetLayer = 0;
    [SerializeField] private Vector3 m_PatrolDirection = Vector3.zero;
    [SerializeField] private float stopTimer = 1f;
    private int patrolState = 0;
    private int maxPatrolState = 3;
    [Header("Attack System")]
    [SerializeField] private float AttackDistance = 1f;
    [SerializeField] private Transform target;
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
    bool isInRange = false;
    [Header("Debug")]
    [SerializeField] private bool DebugMode = false;
    private void DebugLog(string text)
    {
        if (!DebugMode) return;
        Debug.Log(text);
    }
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
    public void ChangeAIState(AIState.State state)
    {
        enemyState.CurrentAIState = state;
    }
    NavMeshAgent agent;

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
        if(m_DefaultTeamId != -1)
        {
            TeamBehaviour.Singleton.TeamManager[m_DefaultTeamId].AddMember(gameObject);
        }
        agent.stoppingDistance = AttackDistance;
    }
    public void setAggro(GameObject originTarget)
    {
        SetTarget(originTarget);
        
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
    Collider excludedTarget = null;
    private void FindTarget()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, lookRadius, targetLayer);
        List<Collider> colliders = new List<Collider>(colliderArray);
        if (target != null && (!target.gameObject.activeInHierarchy || !colliders.Contains(target.GetComponent<Collider>())))
        {
            DebugLog("Target is inactive!");
            excludedTarget = null;
            target = null;
        }
        if (DebugMode)
        {
            string Debug = "Collider List(Before):\n";
            foreach (Collider collider in colliders)
            {
                Debug += collider.gameObject.name + "\n";
            }
            DebugLog(Debug);
        }
        int teamID = TeamBehaviour.Singleton.GetTeamID(gameObject);
        if (teamID != -1)
        {
            colliders.RemoveAll(itemA => TeamBehaviour.Singleton.TeamManager[teamID].TeamColliders.Contains(itemA));
        }
        if(excludedTarget != null)
        {
            colliders.Remove(excludedTarget);
        }
        if(DebugMode)
        {
            string Debug = "Collider List:\n";
            foreach (Collider collider in colliders)
            {
               Debug+= collider.gameObject.name + "\n";
            }
            DebugLog(Debug);
        }
        if (colliders.Count > 0)
        {
            float closestDistance = Mathf.Infinity;
            GameObject closestObject = null;
            ////Find preferred target
            //foreach (Collider collider in colliders)
            //{
            //    float distance = Vector3.Distance(transform.position, collider.transform.position);

            //    if (distance < closestDistance)
            //    {      
            //        DebugLog("Target Object: " + collider.gameObject + "Layer: " + collider.gameObject.layer.ToString() + " / PreferredTargetLayer: " + preferredTargetLayer + "\nPath Reachable? " + agent.path.status.ToString());
            //        if (collider.gameObject.layer == preferredTargetLayer)
            //        {
            //            if (currentTarget != closestObject)
            //            {
            //                closestDistance = distance;
            //                closestObject = collider.gameObject;
            //                DebugLog("IsPrferredTarget");
            //                SetTarget(closestObject);
            //                return;
            //            }
                        
            //        }

            //    }
            //}
            if (target != null) return;

            foreach (Collider collider in colliders)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                if ((collider.gameObject.layer == preferredTargetLayer))
                {
                    closestObject = collider.gameObject;
                    break;
                }
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = collider.gameObject;
                    

                }
            }

            if (closestObject != null)
            {
                DebugLog("Closest Obj:" + closestObject.name);
                if (target == null || target.gameObject.activeInHierarchy == false || target.GetComponent<Collider>().enabled == false)
                {
                    if (currentTarget != closestObject)
                    {
                        SetTarget(closestObject);
                    }

                }
            }

        }
        else
        {
            DebugLog("No Target Found!");
            target = null;
            currentTarget = null;
            isInRange = false;
            excludedTarget = null;
        }
    }
    bool PathIsReachable(Transform targetDestination)
    {
        NavMeshPath path = new NavMeshPath();
        bool hasPath = agent.CalculatePath(targetDestination.position, path);
        DebugLog(path.status.ToString());
        return hasPath/* && path.status == NavMeshPathStatus.PathComplete*/;
    }
    private void SetTarget(GameObject targetObj)
    {
        if (targetObj != gameObject)
        {
            target = targetObj.transform;
            currentTarget = targetObj;
            enemyState.CurrentAIState = enemyState.MainAIState;
            isPatrol = false;
            patrolState = -1;
            agent.SetDestination(target.position);
            agent.speed = chaseSpeed;
            isInRange = true;

        }
    }
    private float cacheExpirationTime = 0.1f; // Time in seconds before cache expires
    private float cacheExpirationTimer; // Timer to track cache expiration
    // Update is called once per frame
    void Update()
    {
        Patrol();
        if (enemyState.CurrentAIState == AIState.State.Attack || enemyState.CurrentAIState == AIState.State.Defend || enemyState.CurrentAIState == AIState.State.Patrol)
        {
            if (Time.time >= cacheExpirationTimer)
            {
                FindTarget();
                cacheExpirationTimer = Time.time + cacheExpirationTime;
            }
            
        } else
        {
            target = null;
            currentTarget = null;
        }
        ChaseTarget();
    }
    bool isPatrol = false;
    Vector3 initialPatrolPos = Vector3.zero;
    private float pauseTimer = 0f;
    private void Patrol()
    {
        //Set State as patrol if the mainState is attack or defend
        if((enemyState.MainAIState == AIState.State.Attack || enemyState.MainAIState == AIState.State.Defend) && target == null && enemyState.CurrentAIState != AIState.State.Patrol)
        {
            enemyState.CurrentAIState = AIState.State.Patrol;
            initialPatrolPos = gameObject.transform.position;
            patrolState = UnityEngine.Random.Range(0, 4);
            SetDestination();
            agent.speed = patrolSpeed;
        }
        //Prevent from Non-Patrol state 
        if (enemyState.CurrentAIState != AIState.State.Patrol) return;
        if (isPatrol)
        {
            if (agent.remainingDistance <= agent.stoppingDistance && agent.destination != initialPatrolPos)
            {
                patrolState++;
                if (patrolState > maxPatrolState)
                {
                    patrolState = 0;
                }
                isPatrol = false;
            }
        }
        if (!isPatrol)
        {
            if (pauseTimer < stopTimer)
            {
                pauseTimer += Time.deltaTime;
            } else
            {
                pauseTimer = 0f;
                SetDestination();
                DebugLog(initialPatrolPos.ToString());
                isPatrol = true;
            }
        }
        
    }
    private void SetDestination()
    {
        switch (patrolState)
        {
            case -1:
                patrolState = 0;
                break;
            case 0:
                agent.SetDestination(initialPatrolPos + new Vector3(m_PatrolDirection.x, 0, m_PatrolDirection.z));
                break;
            case 1:
                agent.SetDestination(initialPatrolPos + new Vector3(m_PatrolDirection.x, 0, -m_PatrolDirection.z));
                break;
            case 2:
                agent.SetDestination(initialPatrolPos + new Vector3(-m_PatrolDirection.x, 0, -m_PatrolDirection.z));
                break;
            case 3:
                agent.SetDestination(initialPatrolPos + new Vector3(-m_PatrolDirection.x, 0, m_PatrolDirection.z));
                break;
        }
    }
    public void ResetExcludedTarget()
    {
        excludedTarget = null;
    }
    private Vector3 LastTargetPosition;
    private void ChaseTarget()
    {
        /* Note: agent.SetDestination() requires network update(Server & Client) when in multiplayer 
         * 
        */
        if (target == null)
        {
            //if(enemyState.CurrentAIState != AIState.State.Patrol)
            //    agent.SetDestination(gameObject.transform.position);
            return;
        }
        if(!agent.pathPending && agent.pathStatus == NavMeshPathStatus.PathPartial && excludedTarget == null && target.gameObject.layer == preferredTargetLayer)
        {
            DebugLog(agent.pathStatus.ToString());
            excludedTarget = target.GetComponent<Collider>();
            target = null;
            currentTarget = null;
            return;
        }
        float distance = Vector3.Distance(target.position, transform.position);
        DebugLog("Distance:" + Mathf.RoundToInt(distance).ToString() + "\nRemaining Distance: " + Mathf.RoundToInt(agent.remainingDistance).ToString());
        if (distance <= lookRadius || isInRange)
        {
            if (LastTargetPosition != target.position)
            {
                LastTargetPosition = target.position;
                agent.SetDestination(target.position);
            }
            if (distance <= agent.stoppingDistance || agent.remainingDistance <= AttackDistance)
            {
                
                //Debug.Log("Attack");
                //Attack the target
                FaceTarget();
                StartAttack();
            }
            else if (distance > agent.stoppingDistance)
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
            DebugLog("Out of Range");
            target = null;
            currentTarget = null;
            LastTargetPosition = Vector3.zero;
            if(AttackCor != null) {
                StopCoroutine(AttackCor);
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
        if (direction.x != 0f || direction.z != 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    private Coroutine AttackCor;
    private void StartAttack()
    {

        AttackTime += Time.deltaTime;

        if (AttackTime >= attackCD)
        {

            AttackCor = StartCoroutine(AttackCoroutine());
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