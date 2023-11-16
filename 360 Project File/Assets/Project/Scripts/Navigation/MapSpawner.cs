using System.Collections.Generic;
using UnityEngine;

public class MapSpawner : MonoBehaviour
{
    [SerializeField]
    private MapFloor[] m_Floors;

    [SerializeField]
    private Material m_PlaneMaterial;

    [SerializeField]
    private SphericalHelper m_SphericalPrefab;

    private Dictionary<string, SphericalHelper> m_SphericalDict;

    private void Awake()
    {
        m_SphericalDict = new();
    }

    private void Start()
    {
        for (int i = 0; i < m_Floors.Length; i++)
            SpawnFloor(m_Floors[i], 2.5f * i);
    }

    private static Mesh CreatePlane(float width, float height)
    {
        return new Mesh()
        {
            vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(width, 0, 0),
                new Vector3(0, 0, height),
                new Vector3(width, 0, height)
            },
            triangles = new int[6]
            {
                0, 2, 1,
                2, 3, 1
            },
            normals = new Vector3[4]
            {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            },
            uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            }
        };
    }

    private void SpawnFloor(MapFloor floor, float yOffset = 0f)
    {
        var width = floor.MapTexture.width / 10f;
        var height = floor.MapTexture.height / 10f;

        var plane = new GameObject(floor.name);
        plane.transform.SetParent(gameObject.transform, false);
        plane.transform.localPosition = new Vector3(0f, yOffset, 0f);

        var meshFilter = plane.AddComponent<MeshFilter>();
        meshFilter.mesh = CreatePlane(width, height);

        var material = Instantiate(m_PlaneMaterial);
        material.mainTexture = floor.MapTexture;

        var renderer = plane.AddComponent<MeshRenderer>();
        renderer.material = material;

        var root = new GameObject("Sphericals");
        root.transform.SetParent(plane.transform, false);
        root.transform.localPosition = new Vector3(0, 0, height);
        foreach (var landmark in floor.Landmarks)
        {
            var spherical = Instantiate(m_SphericalPrefab, root.transform);
            spherical.gameObject.name = landmark.name;
            spherical.Texture = landmark.Texture;
            spherical.transform.SetLocalPositionAndRotation(new Vector3(landmark.Position.x / 10f, 1f, -(landmark.Position.y / 10f)), Quaternion.Euler(landmark.Rotation));
            m_SphericalDict.Add(landmark.name, spherical);
        }
    }
}
