using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRipples : MonoBehaviour
{
    [SerializeField] private GameObject ripplesVFX;
    private Material material;
    [SerializeField] private LayerMask hitLayer;
    [SerializeField] private float DestroyTime = 1f;
    private bool isEffectActive = false;
    private float lerpDuration = 1f; // Duration for the lerp in seconds
    private float lerpTimer = 0f; // Timer for the lerp
    private float initialContactPoint; // Initial contact point for the lerp
    private GameObject currentRipple;
    private void OnTriggerEnter(Collider other)
    {
        if (hitLayer == (hitLayer | (1 << other.gameObject.layer)))
        {
            if (currentRipple == null)
            {
                Vector3 contactPoint = other.ClosestPointOnBounds(transform.position);
                var ripples = Instantiate(ripplesVFX, gameObject.transform.position, Quaternion.identity, transform) as GameObject;
                currentRipple = ripples;
                var psr = ripples.transform.GetComponent<ParticleSystemRenderer>();
                Material uniqueMaterial = new Material(psr.sharedMaterial);
                uniqueMaterial.SetVector("_SphereCenter", contactPoint);
                psr.material = uniqueMaterial;
                material = psr.material;
                material.SetVector("_SphereCenter", contactPoint);
                Destroy(ripples.gameObject, DestroyTime);
            }
        }
    }

    private void Update()
    {
        if (isEffectActive)
        {
            lerpTimer -= Time.deltaTime;
            float t = Mathf.Clamp01(lerpTimer / lerpDuration);
            material.SetFloat("_SphereRadius",Mathf.Lerp(initialContactPoint, 0f, t));

            if (t <= 0f)
            {
                // Lerp completed, reset variables
                isEffectActive = false;
                lerpTimer = 0f;
            }
        }
    }
}
