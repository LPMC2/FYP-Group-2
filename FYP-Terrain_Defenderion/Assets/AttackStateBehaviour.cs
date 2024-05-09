using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackStateBehaviour : MonoBehaviour
{
    NavMeshAgent agent;
    EnemyController enemyController;
    [SerializeField] private State CloseRangeAttackState;
    [SerializeField] private State LongRangeAttackState;
    private AttackState currentState;
    [SerializeField] private StructureManager structureManager;
    [SerializeField] private TeamBehaviour teamBehaviour;
    private GameObject target;
    [SerializeField] private GameObject core;
    [SerializeField] private float distanceToAttact = 100f;

    // Start is called before the first frame update
    void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        enemyController = gameObject.GetComponent<EnemyController>();
    }
    private void SetTarget()
    {
        if(target != null && !target.activeInHierarchy)
        {
            target = null;
        }
        if (target != null) return;
        target = core;
        foreach(StructurePooling structure in structureManager.structurePoolings)
        {
            foreach(GameObject gameObject in structure.structures)
            {
                if (!gameObject.activeInHierarchy) continue;
                if (target != null)
                {
                    float newDistance = Vector3.Distance(transform.position, gameObject.transform.position);
                    float preDistance = Vector3.Distance(transform.position, target.transform.position);
                    if(newDistance < preDistance)
                    {
                        target = gameObject;
                    }
                } else
                    target = gameObject;
            }
        }
        FocusTarget();
    }
    private void FocusTarget()
    {
        if (target == null) return;
        foreach(GameObject enemy in teamBehaviour.TeamManager[1].TeamList)
        {
            if(enemy.layer == LayerMask.NameToLayer("Enemy"))
            {
                float distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance > distanceToAttact) continue;
                EnemyController enemyController = enemy.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.setAggro(target);
                    enemyController.setAggroState(false);
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (enemyController == null || agent == null) return;
        if (enemyController.getTarget() == null) return;
        float Distance = Vector3.Distance(gameObject.transform.position, enemyController.getTarget().position);

        if(Distance >= LongRangeAttackState.ActivationDistance && currentState != AttackState.LongRange)
        {
            agent.stoppingDistance = LongRangeAttackState.AttackDistance;
            enemyController.SetItem(LongRangeAttackState.Weapon);
            enemyController.SetProjectile(LongRangeAttackState.UsingProjectile);
            enemyController.setAttackData(LongRangeAttackState.AttackDamage, LongRangeAttackState.AttackSpeed, LongRangeAttackState.PreAttackCD);
            enemyController.SetAttackAnimation(LongRangeAttackState.AttackAnimation);
            enemyController.SetAttackType(EnemyController.AttackType.Ranged);
            currentState = AttackState.LongRange;
        } else
        if(Distance <= CloseRangeAttackState.ActivationDistance && currentState != AttackState.CloseRange)
        {
            agent.stoppingDistance = CloseRangeAttackState.AttackDistance;
            enemyController.SetItem(CloseRangeAttackState.Weapon);
            enemyController.setAttackData(CloseRangeAttackState.AttackDamage, CloseRangeAttackState.AttackSpeed, CloseRangeAttackState.PreAttackCD);
            enemyController.SetAttackType(EnemyController.AttackType.Melee);
            enemyController.SetProjectile(CloseRangeAttackState.UsingProjectile);
            enemyController.SetAttackAnimation(CloseRangeAttackState.AttackAnimation);
            currentState = AttackState.CloseRange;
        } else
        {
            if(currentState != AttackState.Default)
            {
                currentState = AttackState.Default;
            }
        }
        SetTarget();

        
    }
    public enum AttackState
    {
        Default,
        CloseRange,
        LongRange
    }
    [System.Serializable]
    public class State
    {
        [SerializeField] private AttackState attackState;
        public AttackState AttackState { get { return attackState; } }
        [SerializeField] private float m_ActivationDistance = 0f;
        public float ActivationDistance { get { return m_ActivationDistance; } }
        [SerializeField] private float m_AttackDistance = 0f;
        public float AttackDistance { get { return m_AttackDistance; } }
        [SerializeField] private GameObject m_Weapon;
        public GameObject Weapon
        {
            get { return m_Weapon; }
        }
        [SerializeField] private GameObject usingProjectile;
        public GameObject UsingProjectile { get { return usingProjectile; } }
        [SerializeField] private float attackDamage;
        [SerializeField] private float attackSpeed;
        [SerializeField] private float preAttackCD;
        public float AttackDamage { get { return attackDamage; } }
        public float AttackSpeed { get { return attackSpeed; } }
        public float PreAttackCD { get { return preAttackCD; } }
        [SerializeField] private int attackAnimation;
        public int AttackAnimation { get { return attackAnimation; } }
    }
}
