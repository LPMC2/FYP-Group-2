using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastBehaviour : MonoBehaviour
{

    [SerializeField]
    private WeaponBehaviour weaponBehaviour;
    public WeaponBehaviour WBehaviour { set { weaponBehaviour = value; } }
    private WeaponFeature.RayData rayData;
    [SerializeField]
    private LineRenderer lineRenderer;
    private void Start()
    {
        if(weaponBehaviour == null)
        {
            weaponBehaviour = gameObject.GetComponent<WeaponBehaviour>();
        }
        rayData = weaponBehaviour.RayData;
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
        else
        {
            lineRenderer = gameObject.GetComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.positionCount = 2;
        lineRenderer.material = rayData.RayMaterial;
        lineRenderer.SetPosition(0, transform.position );
        lineRenderer.SetPosition(1, transform.position + transform.forward * rayData.Range);
        lineRenderer.enabled = false;
    }
    List<GameObject> hitTargets = new List<GameObject>();
    private Vector3 CalculateSpreadDirection(Transform fireTransform, float SpreadAngleX, float SpreadAngleY)
    {
        float randomHorizontalAngle = Random.Range(-SpreadAngleX / 2, SpreadAngleX / 2);
        float randomVerticalAngle = Random.Range(-SpreadAngleY / 2, SpreadAngleY / 2);
        Vector3 forward = fireTransform.forward;
        Vector3 up = fireTransform.up;
        Vector3 right = fireTransform.right;
        Quaternion horizontalRotation = Quaternion.AngleAxis(randomHorizontalAngle, up);
        Quaternion verticalRotation = Quaternion.AngleAxis(randomVerticalAngle, right);
        Quaternion spreadRotation = horizontalRotation * verticalRotation;

        // Calculate the spread direction by rotating the transformed camera's right vector with the spread rotation
        Vector3 spreadDirection = spreadRotation * forward;
        return spreadDirection;
    }
    private void FireSingleRay()
    {
        if (weaponBehaviour == null) return;

        //Vector3 raycastOrigin = weaponBehaviour.FirePoint.transform.position + Vector3.up * 0.1f;

        for (int i = 0; i < rayData.BulletCount; i++)
        {
            Vector3 spreadDirection = CalculateSpreadDirection(weaponBehaviour.FirePoint.transform, rayData.HSpreadAngle, rayData.VSpreadAngle);

            // Draw a debug ray to show the direction of spreadDirection
            Debug.DrawRay(weaponBehaviour.FirePoint.transform.position, spreadDirection * rayData.Range, Color.blue, 2f);

            RaycastHit[] hits;
            hits = Physics.RaycastAll(weaponBehaviour.FirePoint.transform.position, spreadDirection, rayData.Range, weaponBehaviour.affectedLayers);
            foreach (RaycastHit hit in hits)
                if (!hitTargets.Contains(hit.collider.gameObject))
                {
                    weaponBehaviour.HitTakeDamage(hit.collider.gameObject);

                        if (rayData.IsPiercing == false)
                        {
                            hitTargets.Clear();
                            break;
                        }
                    
                    hitTargets.Add(hit.collider.gameObject);
                }
        }
        hitTargets.Clear();
    }
    float setLaserTimer = 0f;
    private void CloseLaser()
    {
        setLaserTimer = rayData.FireDuration / 1.5f * rayData.RayClosingTime;
    }
    public void StartFireRay()
    {
        StartCoroutine(RayRenderEnumerator());
    }
    private IEnumerator RayRenderEnumerator()
    {
        float laserTimer = 0f;
        if (setLaserTimer > 0f)
        {
            laserTimer = setLaserTimer;
            setLaserTimer = 0f;
        }
        if (rayData.rayType == WeaponFeature.RayData.RayType.ONESHOT)
        {
            FireSingleRay();
        }
        float initialWidth = lineRenderer.widthMultiplier;
        while (laserTimer < rayData.FireDuration)
        {
            laserTimer += Time.deltaTime;
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }
            if (laserTimer > rayData.RayClosingTime * rayData.FireDuration)
            {
                float curveTime = (laserTimer - rayData.RayClosingTime * rayData.FireDuration) / ((1 - rayData.RayClosingTime) * rayData.FireDuration);
                float widthMultiplier = rayData.RayAnimationCurve.Evaluate(curveTime) * initialWidth;
                lineRenderer.widthMultiplier = widthMultiplier;
            }
            if(rayData.rayType == WeaponFeature.RayData.RayType.CONTINUOUS)
            {
                FireSingleRay();
            }
            yield return null;
        }
        lineRenderer.SetPosition(0, weaponBehaviour.FirePoint.transform.position );
        lineRenderer.SetPosition(1, weaponBehaviour.FirePoint.transform.position + weaponBehaviour.FirePoint.transform.forward * rayData.Range);
        lineRenderer.enabled = false;
        lineRenderer.widthMultiplier = initialWidth;
    }
    /*
    private IEnumerator RayRenderingEnumerator()
    {
        float laserTimer = 0f;
        if (setLaserTimer > 0f)
        {
            laserTimer = setLaserTimer;
            setLaserTimer = 0f;
        }
        float initialWidth = lineRenderer.widthMultiplier;

        // Create a new mesh for the line segment
        Mesh lineMesh = CreateLineMesh();

        // Create an array to store the transformation matrices for each line
        Matrix4x4[] matrices = new Matrix4x4[numberOfLines];

        // Set the initial position and rotation for each line
        for (int i = 0; i < numberOfLines; i++)
        {
            matrices[i] = Matrix4x4.TRS(weaponBehaviour.FirePoint.transform.position, weaponBehaviour.FirePoint.transform.rotation, Vector3.one);
        }

        // Enable GPU instancing for the LineRenderer material
        lineRenderer.material.enableInstancing = true;

        while (laserTimer < rayData.FireDuration)
        {
            laserTimer += Time.deltaTime;
            if (Time.timeScale == 0)
            {
                yield return null;
                continue;
            }
            if (laserTimer > rayData.RayClosingTime * rayData.FireDuration)
            {
                float curveTime = (laserTimer - rayData.RayClosingTime * rayData.FireDuration) / ((1 - rayData.RayClosingTime) * rayData.FireDuration);
                float widthMultiplier = rayData.RayAnimationCurve.Evaluate(curveTime) * initialWidth;

                // Update the widthMultiplier for all LineRenderers using GPU instancing
                lineRenderer.material.SetFloat("_WidthMultiplier", widthMultiplier);
            }

            // Render the lines using Graphics.DrawMeshInstanced
            Graphics.DrawMeshInstanced(lineMesh, 0, lineRenderer.material, matrices);

            yield return null;
        }

        // Reset the widthMultiplier and disable GPU instancing
        lineRenderer.material.SetFloat("_WidthMultiplier", initialWidth);
        lineRenderer.material.enableInstancing = false;

        // Cleanup the line mesh
        Destroy(lineMesh);
    }

    private Mesh CreateLineMesh()
    {
        // Create a new mesh for the line segment
        Mesh lineMesh = new Mesh();

        // Define the vertices and indices for a simple line segment
        Vector3[] vertices = new Vector3[2] { Vector3.zero, Vector3.forward };
        int[] indices = new int[2] { 0, 1 };

        // Assign the vertices and indices to the mesh
        lineMesh.vertices = vertices;
        lineMesh.SetIndices(indices, MeshTopology.Lines, 0);

        return lineMesh;
    }
    */
}
