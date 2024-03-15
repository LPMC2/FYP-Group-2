using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssistBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float m_maxDistance = 1000f;
    [SerializeField] private LayerMask m_contactLayers;
    [SerializeField] private float m_movementSpeed = 1f;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    Vector3 point = Vector3.zero;
    void Update()
    {
        if (!player.activeInHierarchy) return;
        RaycastHit hitPos;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hitPos, m_maxDistance, m_contactLayers))
        {
            Vector3 targetPosition = hitPos.point;
            Vector3 newPosition = Vector3.Lerp(gameObject.transform.position, targetPosition, Time.deltaTime * m_movementSpeed);
            gameObject.transform.position = newPosition;
        }
        else
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, m_maxDistance));
            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, targetPosition, Time.deltaTime * m_movementSpeed);
        }

    }
}
