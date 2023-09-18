using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private LayerMask mask;
    [SerializeField] private float distance = 1.5f;
  
    private PlayerUI playerUI;
    // Start is called before the first frame update
    void Start()
    {
        playerUI = GetComponent<PlayerUI>();
       
    }

    // Update is called once per frame
    void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward * distance);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, mask)) 
        {
            if(hitInfo.collider.GetComponent<Interactable>() != null && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                playerUI.UpdateText(interactable.promptMessage);
                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactable.BaseInteract();
                }
            }
        }
    }
}
