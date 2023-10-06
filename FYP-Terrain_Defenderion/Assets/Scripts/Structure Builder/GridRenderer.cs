using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRenderer : MonoBehaviour
{
    public int gridSizeX = 10; // Number of cells in X direction
    public int gridSizeY = 10; // Number of cells in Y direction
    public float cellSize = 1f; // Size of each cell

    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawGrid();
    }

    private void DrawGrid()
    {
        int numberOfLines = (gridSizeX + 1) + (gridSizeY + 1);

        lineRenderer.positionCount = numberOfLines * 2;

        int index = 0;

        // Draw horizontal lines
        for (int x = 0; x <= gridSizeX; x++)
        {
            float xPos = x * cellSize;
            lineRenderer.SetPosition(index++, new Vector3(xPos, 0f, 0f));
            lineRenderer.SetPosition(index++, new Vector3(xPos, 0f, gridSizeY * cellSize));
        }

        // Draw vertical lines
        for (int y = 0; y <= gridSizeY; y++)
        {
            float yPos = y * cellSize;
            lineRenderer.SetPosition(index++, new Vector3(0f, 0f, yPos));
            lineRenderer.SetPosition(index++, new Vector3(gridSizeX * cellSize, 0f, yPos));
        }
    }
}
