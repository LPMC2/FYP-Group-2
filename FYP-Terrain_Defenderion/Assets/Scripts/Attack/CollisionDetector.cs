using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private LayerMask includedLayerMask = ~0;
    public LayerMask IncludedLayerMask { set { includedLayerMask = value; } }
    public delegate void OnCollisionEvent(bool state);
    public  event OnCollisionEvent OnCollision;
    public GameObject HitEntity { get; private set; }
    [SerializeField] private CollisionType collisionType;
    public void SetCollsiionType(CollisionType collisionType)
    {
        this.collisionType = collisionType;
    }
    public bool isHit = false;
    private bool currentHitState = false;
    private void OnCollisionStay(Collision collision)
    {
        if (collisionType != CollisionType.Collision) return;
        if (collision.transform.IsChildOf(gameObject.transform)) return;
        HitEntity = collision.gameObject;
        isHit = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (collisionType != CollisionType.Trigger) return;
        if (other.transform.IsChildOf(gameObject.transform) || includedLayerMask != (includedLayerMask | (1 << other.gameObject.layer))) return;
        HitEntity = other.gameObject;
        isHit = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collisionType != CollisionType.Collision) return;
        if (collision.transform.IsChildOf(gameObject.transform)) return;
        HitEntity = null;
        isHit = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if (collisionType != CollisionType.Trigger) return;
        if (other.transform.IsChildOf(gameObject.transform) || includedLayerMask != (includedLayerMask | (1 << other.gameObject.layer))) return;
        HitEntity = null;
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