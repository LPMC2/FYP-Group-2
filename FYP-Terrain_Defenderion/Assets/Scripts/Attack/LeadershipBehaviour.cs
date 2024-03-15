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
    private void OnDestroy()
    {
        Destroy(areaEffect);
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
                hitObj = hitPos.collider.gameObject;
            } else
            {
                hitObj = null;
            }
        }
        
        if (playerInput == null && transform.root.gameObject != gameObject && transform.root.GetComponent<InputManager>() != null)
        {
            teamId = teamBehaviour.GetTeamID(transform.root.gameObject);
            playerInput = transform.root.GetComponent<InputManager>();
        }
        if (playerInput.LeftMouseClick)
        {
            areaEffect.transform.position = point;
            if (!isLeading)
            {
                scaleAnimation.StartAnimation();
                areaEffect.transform.localScale = new Vector3(1f, 1f, 1f);
                isLeading = true;
            }
        } else if(!playerInput.LeftMouseClick && isLeading)
        {
            scaleAnimation.StartAnimation();
            isLeading = false;
            Lead();
        }
        if(playerInput.RightMouseClick)
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
            if (isGathering)
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
            SetAIState(gameObject.GetComponent<EnemyController>());
            NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            navMeshAgent.SetDestination(point);
        }
        inRangedTeamMembers.Clear();

    }


}