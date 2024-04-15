using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private bool isDebug = false;
    [SerializeField] private LayerMask includedLayerMask = ~0;
    public LayerMask IncludedLayerMask { set { includedLayerMask = value; } }
    public delegate void OnCollisionEvent(bool state);
    public  event OnCollisionEvent OnCollision;
    [SerializeField] private List<GameObject> m_hitEntities = new List<GameObject>();
    public List<GameObject> HitEntities { get { return m_hitEntities; } }
    [SerializeField] private CollisionType collisionType;
    private void DebugLog(string str)
    {
        if(isDebug)
        {
            Debug.Log(str);
        }
    }
    public void SetCollsiionType(CollisionType collisionType)
    {
        this.collisionType = collisionType;
    }
    public bool isHit = false;
    private bool currentHitState = false;
    private void OnCollisionStay(Collision collision)
    {
        if (collisionType != CollisionType.Collision) return;
        DebugLog("OnHit!" + collision.gameObject);
        if (collision.transform.IsChildOf(gameObject.transform)) return;
        if(!m_hitEntities.Contains(collision.gameObject))
        {
            m_hitEntities.Add(collision.gameObject);
        }
       
        isHit = true;
     
    }
    private void OnTriggerStay(Collider other)
    {
        if (collisionType != CollisionType.Trigger) return;
        DebugLog("OnHit!" + other.gameObject);
        if (other.transform.IsChildOf(gameObject.transform) || includedLayerMask != (includedLayerMask | (1 << other.gameObject.layer))) return;
        if (!m_hitEntities.Contains(other.gameObject))
        {
            m_hitEntities.Add(other.gameObject);
        }
        isHit = true;

    }
    private void OnCollisionExit(Collision collision)
    {
        if (collisionType != CollisionType.Collision) return;
        DebugLog("HitExit!" + collision.gameObject);
        if (collision.transform.IsChildOf(gameObject.transform)) return;
        if(m_hitEntities.Contains(collision.gameObject))
        {
            m_hitEntities.Remove(collision.gameObject);
        }
       
        isHit = false;

    }
    private void OnTriggerExit(Collider other)
    {
        if (collisionType != CollisionType.Trigger) return;
        DebugLog("HitExit!" + other.gameObject);
        if (other.transform.IsChildOf(gameObject.transform) || includedLayerMask != (includedLayerMask | (1 << other.gameObject.layer))) return;
        if (m_hitEntities.Contains(other.gameObject))
        {
            m_hitEntities.Remove(other.gameObject);
        }
        isHit = false;
 
    }
    public void Update()
    {
        if (isHit != currentHitState)
        {
            OnCollision?.Invoke(isHit);
            currentHitState = isHit;
        }
    }
}
public enum CollisionType
{
    Collision,
    Trigger
}