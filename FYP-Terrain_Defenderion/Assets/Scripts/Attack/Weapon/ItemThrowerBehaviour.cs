using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemThrowerBehaviour : MonoBehaviour
{
    [SerializeField]
    private WeaponBehaviour weaponBehaviour;
    public WeaponBehaviour WBehaviour { set { weaponBehaviour = value; } }
    WeaponFeature.ProjectileData data;
    // Start is called before the first frame update
    private float m_destroyPoolTime = 0f;
    void Start()
    {
        if(weaponBehaviour == null)
        {
            weaponBehaviour = gameObject.GetComponent<WeaponBehaviour>();
        }
        data = weaponBehaviour.ProjectileData;
        m_destroyPoolTime = data.ThrowItem.GetComponent<Projectile>().MaxTime;
        //if (GameObjectExtension.GetGameObjectWithTagFromChilds(transform.root.gameObject, "BasePool") );
        objectPool.Initialize(data.ThrowItem, 10, transform.root);
    }
    public void OnDestroy()
    {
        Destroy(objectPool.basePool, m_destroyPoolTime);
    }
    [SerializeField]
    ObjectPool objectPool = new ObjectPool(); 
    public void Shoot()
    {

        GameObject projectileInstance = objectPool.GetObject(true, weaponBehaviour.FirePoint.transform.position, Quaternion.LookRotation(weaponBehaviour.FirePoint.transform.forward));
            //Instantiate(data.ThrowItem, weaponBehaviour.FirePoint.transform.position, Quaternion.LookRotation(weaponBehaviour.FirePoint.transform.forward));
        Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileInstance.transform.rotation = weaponBehaviour.FirePoint.rotation;
            projectileScript.InitializeProjectile(projectileInstance.transform.forward * data.ProjSpeed * Time.deltaTime, data.ProjSpeed, weaponBehaviour.damage, data.Type, weaponBehaviour.owner, false, weaponBehaviour.affectedLayers);
            projectileScript.SetAOE(data.isAOE, data.AOERadius);
        } else
        {
#if UNITY_EDITOR
            Debug.LogError("The Target Throwing Object is NOT a Projectile!");
#endif
        }
    }
}
