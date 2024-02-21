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
    private bool isAggro = false;
    [SerializeField] private GameObject itemPlaceholder;
    [SerializeField] private float AttackDistance = 1f;
    [SerializeField] private Transform target;
    [SerializeField] public AttackType attackType;
    public void SetAttackType(AttackType attackType)
    {
        this.attackType = attackType;
    }
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
    float movementThreshold = 0.1f;
    [Header("Animation & Sound")]
    [SerializeField] private GameObject AnimateObject;
    [SerializeField] private AudioClip AttackSound;
    [SerializeField] private AudioClip DeathSound;
    [SerializeField] private AnimationBehaviour attackAnimations;
    [SerializeField] private AnimationBehaviour movementAnimations;
    [SerializeField] private AnimationBehaviour deathAnimations;
    [SerializeField] private int currentAttackAnimation = -1;
    [SerializeField] private bool isControlled = false;
    public bool IsControl { set { isControlled = value; } }
    public void SetAttackAnimation(int value) { currentAttackAnimation = value; }
    private AudioSource audioSource;
    bool isInRange = false;
    private bool isActive = true;
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
    public void Respawn()
    {
        isActive = true;
    }
    public void Death()
    {
        
        deathAnimations.StartAnimationRandom(enemyAnimator, 1f);
        target = null;
        currentTarget = null;
        isAggro = false;
        agent.SetDestination(gameObject.transform.position);
        isActive = false;
    }
    public void SetItem(GameObject target)
    {
        if (target == null) return;
        GameObjectExtension.RemoveAllObjectsFromParent(itemPlaceholder.transform);
        GameObject targetItem = Instantiate(target, itemPlaceholder.transform.position, Quaternion.identity, itemPlaceholder.transform);
        targetItem.transform.localPosition = target.transform.position;
        targetItem.transform.localRotation = target.transform.rotation;
    }
    public void SetProjectile(GameObject projectile)
    {
        projectileObject = projectile;
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
        if(state != AIState.State.Patrol)
        {
            isPatrol = false;
        }
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
        if (!isAggro)
        {
            SetTarget(originTarget);
            isAggro = true;
        }
        
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
            currentTarget = null;
            isAggro = false;
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
        if (isAggro) return;
        int teamID = TeamBehaviour.Singleton.GetTeamID(gameObject);
        colliders = TeamBehaviour.Singleton.RemoveFriendlyMembers(teamID ,colliders);
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
            if(agent.isActiveAndEnabled)
                agent.SetDestination(target.position);
            agent.speed = chaseSpeed;
            isInRange = true;
            agent.stoppingDistance = AttackDistance;
            isMoving = false;
        }
    }
    private float cacheExpirationTime = 0.1f; // Time in seconds before cache expires
    private float cacheExpirationTimer; // Timer to track cache expiration
    // Update is called once per frame
    private bool isMoving = false;
    float movementSpeed = 0f;
    void Update()
    {
        if (isControlled && agent.remainingDistance <= agent.stoppingDistance) { isControlled = false; }
        if (!isActive || isControlled) return;
        //if (target != null) {
        //    TeamBehaviour.Singleton.isOwnTeam(gameObject, target.GetComponent<Collider>());
        //    target = null;
        //    currentTarget = null;
        //}
        movementSpeed = agent.velocity.magnitude;
        if (agent.velocity.magnitude > movementThreshold && isMoving == false)
        {
            if (agent.velocity.magnitude > 0.1 && agent.velocity.magnitude <= patrolSpeed)
            {
                movementAnimations.StartAnimationConstant(enemyAnimator, 1, 1f);
            } else if(agent.velocity.magnitude > 0.4 && agent.velocity.magnitude <= chaseSpeed)
            {
                movementAnimations.StartAnimationConstant(enemyAnimator, 2, 1f);
            } else
            {
                movementAnimations.StartAnimationConstant(enemyAnimator, 3, 1f);
            }
            isMoving = true;
        } else if(agent.velocity.magnitude <= movementThreshold && isMoving)
        {
            movementAnimations.StartAnimationConstant(enemyAnimator, 0, 1f);
            isMoving = false;
        }
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
            agent.stoppingDistance = 1f;
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
        if (target == null || TeamBehaviour.Singleton.GetTeamID(target.gameObject) == TeamBehaviour.Singleton.GetTeamID(gameObject))
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
                AttackTime = 0f;
                if(AttackCor != null)
                StopCoroutine(AttackCor);
                if (AnimateObject != null)
                {
                    //if (!enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                    //{
                    //    StartCoroutine(PlayWalkAnimation());
                    //}
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
        if (direction.x != 0f && direction.z != 0f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }
    private Coroutine AttackCor;
    private void StartAttack()
    {

        AttackTime += Time.deltaTime;

        if (AttackTime >= attackCD && target != null)
        {

            AttackCor = StartCoroutine(AttackCoroutine());
            AttackTime = 0;
        }
    }

    private IEnumerator AttackCoroutine()
    {
        if (target == null) { StopCoroutine(AttackCor); }
        if (enemyAnimator != null && currentAttackAnimation != -1 && currentAttackAnimation < attackAnimations.GetAnimationLength())
        {
            // Play the "Walk" animation clip
            attackAnimations.StartAnimationConstant(enemyAnimator, currentAttackAnimation, attackCD);

        }
        if (attackType == AttackType.Melee)
        {
                if (DeathSound != null && audioSource != null)
                {
                    audioSource.clip = AttackSound;
                    audioSource.Play();
                }

            yield return new WaitForSeconds(preAttackCD);
            if (target != null)
            {
                IDamageable damageable = target.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }

                //Set Aggro to Target
                EnemyController enemy = target.GetComponent<EnemyController>();
                if (enemy != null) { enemy.setAggro(gameObject); }
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
            if (projectileType != ProjectileType.Raycast)
            {
                if (projectileObject != null)
                {
                    GameObject projectileInstance = Instantiate(projectileObject, transform.position + transform.TransformDirection(ProjectileOffset), Quaternion.identity);
                    Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
                    if (projectileScript != null && target!=null)
                    {
                        Vector3 relativePos = target.transform.position - projectileInstance.transform.position;
                        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
                        projectileInstance.transform.rotation = rotation;
                        Vector3 direction = target.position - transform.position;
                        direction.Normalize();
                        projectileScript.InitializeProjectile(direction, ProjectileSpeed, damage, projectileType, gameObject);

                    }
                }
            } else
            {
                FireLaser();
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
    private void FireLaser()
    {
        LaserBehaviour laserBehaviour = gameObject.GetComponent<LaserBehaviour>();
        if (laserBehaviour != null)
        {
            laserBehaviour.fire(damage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (DebugMode)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, lookRadius);
        }
    }
}