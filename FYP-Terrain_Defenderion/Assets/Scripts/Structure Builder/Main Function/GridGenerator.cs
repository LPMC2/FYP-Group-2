using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public int numRows = 25;
    public int numColumns = 25;
    public int numHeight = 25;
    public float cellSize = 0.5f;
    public GameObject BuildBlock(GameObject block, Transform parent, Transform shootingPoint, float cellSize, float range)
    {
        GameObject targetBlock = new GameObject();
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, range))
        {
            if (hitInfo.transform.CompareTag("Grid"))
            {
                Vector3 spawnPosition = CalculateSpawnPosition(hitInfo.point, hitInfo.normal, cellSize);
                if (IsValidPosition(spawnPosition))
                {
                   targetBlock = Instantiate(block, spawnPosition, Quaternion.identity, parent);
                }
            }
            else
            {
                Vector3 spawnPosition = CalculateSpawnPosition(hitInfo.point, Vector3.zero, cellSize);
                if (IsValidPosition(spawnPosition))
                {
                    targetBlock = Instantiate(block, spawnPosition, Quaternion.identity, parent);
                }
            }
        }
        return targetBlock;
    }
    public void DestroyBlock(Transform shootingPoint, float range)
    {
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out RaycastHit hitInfo, range))
        {
            if (hitInfo.transform.tag == "Grid")
            {
                Destroy(hitInfo.transform.gameObject);
            }
        }
    }

    private static Vector3 CalculateSpawnPosition(Vector3 hitPoint, Vector3 hitNormal, float cellSize)
    {
        float x = Mathf.RoundToInt((hitPoint.x + hitNormal.x) * cellSize / 2);
        float y = Mathf.RoundToInt((hitPoint.y + hitNormal.y) * cellSize / 2);
        float z = Mathf.RoundToInt((hitPoint.z + hitNormal.z) * cellSize / 2);

        return new Vector3(x, y, z);
    }

    private bool IsValidPosition(Vector3 spawnPosition)
    {

        // Check if the spawn position is within the valid range
        return spawnPosition.x > -numRows/2 && spawnPosition.x < numRows/2 &&
               spawnPosition.y >= 0 && spawnPosition.y < numHeight &&
               spawnPosition.z > -numColumns && spawnPosition.z < numColumns;
    }
}
