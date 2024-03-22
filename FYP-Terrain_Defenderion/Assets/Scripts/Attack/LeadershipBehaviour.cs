using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
/* Note from Liam:
 * Just a heads up,
 * This function is only a simple version of Gather/Lead using Nav Mesh agant functions, it will be enhanced at some point
*/
public class LeadershipBehaviour : MonoBehaviour
{
    [SerializeField] private bool useInput = true;
    private InputManager playerInput;
    private TeamBehaviour teamBehaviour;
    private int teamId;
    [SerializeField] private float m_GatherRadius = 5f;
    [SerializeField] private float m_ActivateDistance = 20f;
    [SerializeField] private LayerMask gather_ContactLayer;
    [SerializeField] private LayerMask lead_ContactLayer;
    [SerializeField] private GameObject pointedAreaEffectPrefab;
    private float cacheExpirationTime = 0.1f; // Time in seconds before cache expires
    private float cacheExpirationTimer; // Timer to track cache expiration
    private List<Collider> inRangedTeamMembers = new List<Collider>();
    private bool isLeading = false;
    private bool isGathering = false;
    GameObject areaEffect;
    ScaleAnimationCoroutineBehaviour scaleAnimation;
    private PlayerManager playerManager;
    [Header("Targeted Lead Settings")]
    [SerializeField] private Material m_targetedMaterial;
    private MaterialBehaviour targetMatBehaviour;
    private GameObject owner;
    private void OnDestroy()
    {
        Destroy(areaEffect);
        if(hitObj != null)
        {
            SetMaterial(false);
        }
    }
    private void OnEnable()
    {
        owner = gameObject.transform.root.gameObject;
    }
    private void Start()
    {
        teamBehaviour = TeamBehaviour.Singleton;
        areaEffect = Instantiate(pointedAreaEffectPrefab, transform.position, Quaternion.identity);
        scaleAnimation = areaEffect.transform.GetChild(0).GetComponent<ScaleAnimationCoroutineBehaviour>();
        playerManager = transform.root.GetComponent<PlayerManager>();
        playerManager.IsRig = false;
    }
    Vector3 point = Vector3.zero;
    GameObject hitObj;
    private void Update()
    {
        RaycastHit hitPos;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hitPos, m_ActivateDistance, lead_ContactLayer))
        {
            point = hitPos.point;
            if (hitPos.collider.GetComponent<IDamageable>() != null)
            {
                if (!TeamBehaviour.Singleton.isOwnTeam(transform.root.gameObject, hitPos.collider))
                {
                    SetMaterial(false);
                    hitObj = hitPos.collider.gameObject;
                    SetMaterial(true);
                }
            }  else
            {
                if (hitObj != null)
                {
                    SetMaterial(false);
                }
                hitObj = null;
            }
        }
        else
        {
            if (hitObj != null)
            {
                SetMaterial(false);
            }
            hitObj = null;
        }

        if (playerInput == null && transform.root.gameObject != gameObject && transform.root.GetComponent<InputManager>() != null)
        {
            teamId = teamBehaviour.GetTeamID(transform.root.gameObject);
            playerInput = transform.root.GetComponent<InputManager>();
        }
        if (useInput && playerInput.LeftMouseClick)
        {
            areaEffect.transform.position = point;
            if (!isLeading)
            {
                scaleAnimation.StartAnimation();
                areaEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                isLeading = true;
            }
        } else if(useInput && !playerInput.LeftMouseClick && isLeading)
        {
            scaleAnimation.StartAnimation();
            isLeading = false;
            Lead();
        }
        if(useInput && playerInput.RightMouseClick)
        {
            SearchingFriendly();
            if (!isGathering)
            {
                scaleAnimation.StartAnimation();
                isGathering = true;
            }
            areaEffect.transform.position = playerInput.transform.position;
            areaEffect.transform.localScale = new Vector3(m_GatherRadius, 1f, m_GatherRadius);
        } else
        {
            if (useInput && isGathering)
            {
                isGathering = false;
                if (areaEffect != null)
                {
                    scaleAnimation.StartAnimation();
                }
            }
        }
        Gather();
    }
    private void SetMaterial(bool state)
    {
        if (hitObj == null) return;
         targetMatBehaviour = hitObj.GetComponent<MaterialBehaviour>();
        if(targetMatBehaviour == null) { targetMatBehaviour = hitObj.AddComponent<MaterialBehaviour>(); }
        if(state)
        {

            targetMatBehaviour.SetMaterial(m_targetedMaterial);
        } else
        {
            targetMatBehaviour.ResetMat();
            targetMatBehaviour = null;
        }
    }
    private void SearchingFriendly()
    {
        if (Time.time >= cacheExpirationTimer)
        {
            SearchFriendly();
            cacheExpirationTimer = Time.time + cacheExpirationTime;
        }
    }
    private void SearchFriendly()
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, m_GatherRadius, gather_ContactLayer);
        inRangedTeamMembers = new List<Collider>(colliderArray);
        inRangedTeamMembers = TeamBehaviour.Singleton.RemoveUnFriendlyMembers(teamId, inRangedTeamMembers);

    }
    private Vector3 GetRandomPosition(GameObject target)
    {
        Vector3 newPos = new Vector3(target.transform.position.x + Random.Range(-m_GatherRadius, m_GatherRadius), target.transform.position.y, target.transform.position.z + Random.Range(-m_GatherRadius, m_GatherRadius));
        return newPos;
    }
    private void Gather()
    {

            foreach (Collider gameObject in inRangedTeamMembers)
            {
            EnemyController enemyController = gameObject.GetComponent<EnemyController>();
            enemyController.ResetTarget();
                NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
                navMeshAgent.SetDestination(transform.root.position);
            }
        
    }
    private void SetAIState(EnemyController enemyController)
    {
        enemyController.ChangeAIState(AIState.State.Attack);
        enemyController.IsControl = true;
    }
    private void SetAIAggro(EnemyController enemyController, GameObject target)
    {
        enemyController.setAggro(target);
    }
    private void Lead()
    {

        foreach (Collider gameObject in inRangedTeamMembers)
        {
            EnemyController enemyController = gameObject.GetComponent<EnemyController>();
            SetAIState(enemyController);
            if (hitObj == null)
            {
                NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
                navMeshAgent.SetDestination(point);
            } else
            {
                SetAIAggro(enemyController, hitObj);
                SetMaterial(false);
            }
        }
        inRangedTeamMembers.Clear();

    }


}