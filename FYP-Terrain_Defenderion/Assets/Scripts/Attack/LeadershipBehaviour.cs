using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class LeadershipBehaviour : MonoBehaviour
{
    private InputManager playerInput;
    private TeamBehaviour teamBehaviour;
    private int teamId;
    [SerializeField] private float m_GatherRadius = 5f;
    [SerializeField] private float m_ActivateDistance = 20f;
    [SerializeField] private LayerMask gather_ContactLayer;
    [SerializeField] private LayerMask lead_ContactLayer;
    private float cacheExpirationTime = 0.1f; // Time in seconds before cache expires
    private float cacheExpirationTimer; // Timer to track cache expiration
    private List<Collider> inRangedTeamMembers = new List<Collider>();
    private void Start()
    {
        teamBehaviour = TeamBehaviour.Singleton;
    }
    private void OnEnable()
    {
        if (transform.root.gameObject != gameObject)
        {
            teamId = teamBehaviour.GetTeamID(transform.root.gameObject);
            playerInput = transform.root.GetComponent<InputManager>();
        }
    }
    private void Update()
    {
        if (Time.time >= cacheExpirationTimer)
        {
            SearchFriendly();
            cacheExpirationTimer = Time.time + cacheExpirationTime;
        }
        if (playerInput.LeftMouseClick)
        {
            Lead();
        }
        if(playerInput.RightMouseClick)
        {
            Gather();
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
        Vector3 newPos = new Vector3(Random.Range(-m_GatherRadius, m_GatherRadius), target.transform.position.y, Random.Range(-m_GatherRadius, m_GatherRadius));
        return newPos;
    }
    private void Gather()
    {
        foreach(Collider gameObject in inRangedTeamMembers)
        {
            NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
            
        }


    }
    private void Lead()
    {

        RaycastHit hitPos;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hitPos, m_ActivateDistance, lead_ContactLayer))
        {
            Vector3 point = hitPos.point;

        }
    }


}
