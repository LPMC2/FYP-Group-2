using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimAssistBehaviour : MonoBehaviour
{
    [SerializeField] private float m_maxDistance = 100;
    [SerializeField] private LayerMask m_contactLayers;   
    // Start is called before the first frame update
    public Quaternion Rotation { get; private set; }
    void Start()
    {
        
    }

    // Update is called once per frame
    Vector3 point = Vector3.zero;
    void Update()
    {

        RaycastHit hitPos;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)), out hitPos, m_maxDistance, m_contactLayers))
        {
            point = hitPos.point;
        }
        Vector3 direction = point - transform.root.position;
        Rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
