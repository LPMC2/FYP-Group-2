using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private bool m_isTravelAtStart = false;
    [SerializeField]
    private bool m_isPiercing = false;
    [SerializeField]
    [Tooltip("This unityevent will be invoked and use the collided gameobject as the parameter when hit")]
    private UnityEvent<GameObject> m_HitFunction;
    [SerializeField]
    private LayerMask m_HitLayer;
    public GameObject ParticleEffect;
    [SerializeField]
    private float damage = 1f;

    private GameObject target;
    private LayerMask obstacleMask;
    private GameObject enemy;
    [SerializeField]
    private float speed = 1f;
    private Rigidbody rb;
    private GameObject nearestDesturctable;
    [SerializeField]
    private float AOE = -1f;
    [SerializeField]
    private float maxTime = 10f;
    public float MaxTime { get { return maxTime; } }
    [SerializeField]
    private ProjectileType projectileType;
    public ProjectileType Type => projectileType;
    private Vector3 direction;
    private GameObject owner;
    private bool isDestroyAfterCollision = false;
    private void Start()
    {
        /*
        if(m_isTravelAtStart)
        {
            InitializeProjectile(gameObject.transform.forward,speed);
        }
        */
    }
    private void OnEnable()
    {
        if (m_isTravelAtStart)
        {
            InitializeProjectile(gameObject.transform.forward, speed);
        }
    }
    public void setDestructable(GameObject targetobject)
    {
        nearestDesturctable = targetobject;
    }
    private void FixedUpdate()
    {

        // Apply force only when in StraightForward state
        if (projectileType == ProjectileType.StraightForward)
        {
           
        }
    }
    public void SetSpeed(float Speed)
    {
        speed = Speed;
    }
    public void SetDamage(float damage)
    {
        this.damage = damage;
    }
    public void SetAOE(bool isAoe, float Aoe)
    {
        if(isAoe == true)
        {
            AOE = Aoe;
        } else
        {
            AOE = -1f;
        }
    }
    public void SetTarget(GameObject target, GameObject enemy)
    {
        this.target = target;
        this.enemy = enemy;
    }

    public void SetObstacleMask(LayerMask obstacleMask)
    {
        this.obstacleMask = obstacleMask;
    }

    public void InitializeProjectile(Vector3 direction,float speedMultiplier, float damageMultiplier = default, ProjectileType projectileType = default, GameObject owner = null, bool DestroyObject = false, LayerMask affectedLayer = default)
    {
        if(projectileType != default)
        {
            this.projectileType = projectileType;
        }
        if(damageMultiplier != default)
        {
            damage = damageMultiplier;
        }
        if (affectedLayer != default)
        {
            m_HitLayer = affectedLayer;
        }
        if (owner != null)
        {
            //Debug.Log(owner);
            this.owner = owner;
        }
        this.direction = direction;
         speed = speedMultiplier;
        switch (this.projectileType)
        {
            case ProjectileType.StraightForward:
                ShootStraight();
                break;
            case ProjectileType.InstantForce:
                ShootWithInstantForce();
                break;
        }
        isDestroyAfterCollision = DestroyObject;
        Debug.Log(DestroyObject);
        if (DestroyObject)
            Destroy(gameObject, maxTime);
        else
            GameObjectExtension.DelayEventInvoke(this, () =>
            {
                gameObject.SetActive(false);
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            },
            maxTime);
            GameObjectExtension.DisableFromTime(this, gameObject, maxTime);
    }
    private bool IsLayerInMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }
    public void ShootStraight()
    {
        if (projectileType == ProjectileType.StraightForward)
        {
            StartCoroutine(MoveStraight());
        }
    }

    public void ShootWithInstantForce()
    {
        
        if (projectileType == ProjectileType.InstantForce)
        {
            rb = GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
           
        }
    }

    private IEnumerator MoveStraight()
    {
        rb = GetComponent<Rigidbody>();

            Vector3 dampeningDirection = rb.velocity.normalized * -1.0f;
            rb.AddForce(dampeningDirection * 10);
        
        while (true)
        {

            float timeElapsed = 0f;
            rb.useGravity = false;
            while (timeElapsed < maxTime)
            {
                rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.VelocityChange);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsLayerInMask(other.gameObject.layer, m_HitLayer) || other.gameObject == owner)
        {
            return;
        }
        if (ParticleEffect != null)
        {
            GameObject particle = Instantiate(ParticleEffect, gameObject.transform.position, Quaternion.identity);
            Destroy(particle, 3f);
        }
        m_HitFunction.Invoke(other.gameObject);
        if (AOE == -1)
        {
            IDamageable iDamageable = other.GetComponent<IDamageable>();
            if (iDamageable != null)
            {
                //Debug.Log("HIt");
                iDamageable.TakeDamage(damage, owner);
            }
        } else
        {
            CastDamageSphere();
        }
        if(TeamBehaviour.Singleton != null)
            if (isOwnTeam(owner, other)) return;
        EnemyController enemy = other.GetComponent<EnemyController>();
        if (enemy != null && owner != null) { enemy.setAggro(owner); }
        if (!m_isPiercing)
        {
            if (isDestroyAfterCollision)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
    }
    public void CastDamageSphere()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, AOE, m_HitLayer);
        
        foreach (Collider collider in colliders)
        {
            Debug.Log(collider);
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, owner);
            }
        }
    }
    private bool isOwnTeam(GameObject origin, Collider target)
    {
        return TeamBehaviour.Singleton.isOwnTeam(origin, target);
    }


}
public enum ProjectileType
{
    Default,
    StraightForward,
    InstantForce,
    Raycast
}