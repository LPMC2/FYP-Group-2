using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridContact : MonoBehaviour
{
    [SerializeField]
    private LayerMask GridLayer;
    SphereCollider sphereCollider;
    //private void Start()
    //{
    //    sphereCollider = GetComponent<SphereCollider>();
    //    Collider[] hitColliders;
    //    hitColliders = Physics.OverlapSphere(transform.position, sphereCollider.radius, GridLayer, QueryTriggerInteraction.Collide); // Should probably add layermask and a triggerquery

    //    for (int i = hitColliders.Length - 1; i > -1; i--)
    //    {
    //        ObjectComponentState(hitColliders[i], true);
    //    }
    //}

    //private void OnTriggerEnter(Collider other)
    //{
    //    ObjectComponentState(other, true);
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    ObjectComponentState(other, false);
    //}

    //private void ObjectComponentState(Collider other, bool state)
    //{
    //    Debug.Log(other);
    //    other.GetComponent<MonoBehaviour>().enabled = state;
    //    other.gameObject.SetActive(state);
    //}
}
