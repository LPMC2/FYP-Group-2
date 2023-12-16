using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(LineRenderer))]
public class LaserBehaviour : MonoBehaviour
{
    private float storeViewRadius = 0;
    [SerializeField] private GameObject laserOriginGameObject;
    [SerializeField] private float laserLength;
    [SerializeField] private Vector3 laserOffset;
    [SerializeField] private AnimationCurve laserCurve;
    [SerializeField] private float laserClosingTime = 0.5f;
    [SerializeField] private Material material;
    [SerializeField] private float fireTime;
    [SerializeField] private float preFireCD;
    [SerializeField] private AudioClip laserSound;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private bool isPiercing = true;
    [SerializeField] private bool canHitMultipleTimes = true;
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private float damage;

    private AudioSource audioSource;
    private void Start()
    {
        if(laserOriginGameObject == null)
        {
            laserOriginGameObject = gameObject;
        }
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        } else
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        
        lineRenderer.positionCount = 2;
        lineRenderer.material = material;
        lineRenderer.SetPosition(0, transform.position + laserOffset);
        lineRenderer.SetPosition(1, transform.position + laserOffset + transform.forward * laserLength);
        lineRenderer.enabled = false;

        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.clip = laserSound;
    }
    public void fire(float damageMultiplier = 1f)
    {
        //Damage Multiplier
        damage *= damageMultiplier;

        // Start a timer for preFireCD seconds before firing the laser
        StartCoroutine(FireLaserAfterDelay(preFireCD));
    }

    private IEnumerator FireLaserAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Enable the line renderer
        lineRenderer.enabled = true;
        audioSource.PlayOneShot(laserSound);
        // Fire the laser for fireTime seconds
        StartCoroutine(FireLaserForDuration(fireTime));

        // Set the enemy's view radius back to its original value after firing the laser
        yield return new WaitForSeconds(fireTime);

    }
    List<GameObject> hitObjects = new List<GameObject>();
private IEnumerator FireLaserForDuration(float duration)
    {
        float laserTimer = 0f;
        float initialWidth = lineRenderer.widthMultiplier;
        while (laserTimer < duration)
        {
            laserTimer += Time.deltaTime;
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }
            // Fire a raycast with length of laserLength
            RaycastHit[] hits;

            Vector3 raycastStart = laserOriginGameObject.transform.position + laserOffset;
            Vector3 raycastDirection = transform.forward;
#if UNITY_EDITOR
            Debug.DrawRay(raycastStart, raycastDirection * laserLength, Color.red, 2f);
#endif

            float time = laserTimer / duration;
            if (laserTimer > laserClosingTime * duration)
            {
                float curveTime = (laserTimer - laserClosingTime * duration) / ((1- laserClosingTime) * duration);
                float widthMultiplier = laserCurve.Evaluate(curveTime) * initialWidth ;
                lineRenderer.widthMultiplier = widthMultiplier;
            }
            hits = Physics.RaycastAll(raycastStart, raycastDirection, laserLength, hitLayerMask);

            if (hits.Length > 0)
            {
                foreach (RaycastHit hit in hits)
                {
                    if (hitObjects.Contains(hit.collider.gameObject))
                    {
                        continue;
                    }

                    HitTakeDamage(hit.collider.gameObject);
                    hitObjects.Add(hit.collider.gameObject);

                    // If the raycast hits an object, do something


                    if (!isPiercing)
                    {
                        lineRenderer.SetPosition(0, raycastStart);
                        lineRenderer.SetPosition(1, hit.point);
                        break;
                    }
                }
                if (isPiercing)
                {
                    lineRenderer.SetPosition(0, raycastStart);
                    lineRenderer.SetPosition(1, raycastStart + raycastDirection * laserLength);
                }
            }
            else
            {
                // If the raycast doesn't hit anything, update the end point of the line renderer to the maximum range of the raycast
                lineRenderer.SetPosition(0, raycastStart);
                lineRenderer.SetPosition(1, raycastStart + raycastDirection * laserLength);
            }
            if(canHitMultipleTimes)
            hitObjects.Clear();
            yield return null;
        }
        if (!canHitMultipleTimes)
            hitObjects.Clear();
        // Reset the line renderer to its original position and disable it
        lineRenderer.SetPosition(0, transform.position + laserOffset);
        lineRenderer.SetPosition(1, transform.position + laserOffset + transform.forward * laserLength);
        lineRenderer.enabled = false;
        lineRenderer.widthMultiplier = initialWidth;

    }
    private void HitTakeDamage(GameObject hit)
    {
        Debug.Log("hit");
        HealthBehaviour healthBehaviour = hit.GetComponent<HealthBehaviour>();
        if (healthBehaviour != null)
        {
            healthBehaviour.TakeDamage(damage);
        }
    }
}
