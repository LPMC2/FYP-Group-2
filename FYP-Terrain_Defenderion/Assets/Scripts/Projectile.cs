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
    private UnityEvent m_HitFunction;
    public GameObject ParticleEffect;
    private float damage;
    private GameObject target;
    private LayerMask obstacleMask;
    private GameObject enemy;
    [SerializeField]
    private float speed = 1f;
    private Rigidbody rb;
    private GameObject nearestDesturctable;
    private float AOE;
    [SerializeField]
    private float maxTime = 10f;
    [SerializeField]
    private ProjectileType projectileType;
    public ProjectileType Type => projectileType;
    private Vector3 direction;
    private void Start()
    {
        if(m_isTravelAtStart)
        {
            InitializeProjectile(gameObject.transform.forward,speed);
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

    public void InitializeProjectile(Vector3 direction,float speedMultiplier, ProjectileType projectileType = default)
    {
        if(projectileType != default)
        {
            this.projectileType = projectileType;
        }
        this.direction = direction;
         speed *= speedMultiplier;
        switch (this.projectileType)
        {
            case ProjectileType.StraightForward:
                ShootStraight();
                break;
            case ProjectileType.InstantForce:
                ShootWithInstantForce();
                break;
        }
        Destroy(gameObject, maxTime);
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

            rb.AddForce(direction * speed, ForceMode.Impulse);
           
        }
    }

    private IEnumerator MoveStraight()
    {
        
        while (true)
        {
            rb = GetComponent<Rigidbody>();

            float timeElapsed = 0f;
            rb.useGravity = false;
            while (timeElapsed < maxTime)
            {
                rb.AddForce(direction * speed * Time.deltaTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
       
    }

    private void OnTriggerEnter(Collider other)
    {
        m_HitFunction.Invoke();
      if(m_isPiercing)
        {
            Destroy(gameObject);
        }
    }



}
public enum ProjectileType
{
    Default,
    StraightForward,
    InstantForce
}