using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemThrowerBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject throwItem;
    [SerializeField] private float ProjectileSpeed;
    [SerializeField] private Vector3 ProjectileOffset;
    [SerializeField] private float damage;
    [SerializeField] private bool isAreaDamage = false;
    [SerializeField] private float AOERadius = 1f;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private ProjectileType projectileType;
    private GameObject owner;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.transform.parent != null)
        owner = gameObject.transform.parent.GetComponent<OriginManager>().OriginGameObject;
    }
    public void shoot()
    {
       
        GameObject projectileInstance = Instantiate(throwItem, firePoint.transform.position + transform.TransformDirection(ProjectileOffset), Quaternion.LookRotation(firePoint.transform.forward));
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.InitializeProjectile(gameObject.transform.forward, ProjectileSpeed, damage, projectileType, owner);
            projectileScript.SetAOE(isAreaDamage, AOERadius);
        }
    }
}
