using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public delegate void OnCollisionEvent(bool state);
    public  event OnCollisionEvent OnCollision;
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
        isHit = true;
    }
    private void OnTriggerStay(Collider other)
    {
        if (collisionType != CollisionType.Trigger) return;
        if (other.transform.IsChildOf(gameObject.transform)) return;
        isHit = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collisionType != CollisionType.Collision) return;
        if (collision.transform.IsChildOf(gameObject.transform)) return;
        isHit = false;
    }
    private void OnTriggerExit(Collider other)
    {
        if (collisionType != CollisionType.Trigger) return;
        if (other.transform.IsChildOf(gameObject.transform)) return;
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