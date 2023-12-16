using UnityEngine;
using System.Collections.Generic;

public class Octree
{
    private OctreeNode rootNode;

    public Octree(Bounds bounds, int maxObjectsPerNode, int maxLevels)
    {
        rootNode = new OctreeNode(bounds, 0, maxObjectsPerNode, maxLevels);
    }

    public void Insert(GameObject obj)
    {
        rootNode.Insert(obj);
    }

    public void Clear()
    {
        rootNode.Clear();
    }

    public void ActivateObjectsInRange(Vector3 position, float range)
    {
        rootNode.ActivateObjectsInRange(position, range);
    }

    private class OctreeNode
    {
        private Bounds bounds;
        private int level;
        private int maxObjectsPerNode;
        private int maxLevels;
        private OctreeNode[] children;
        private List<GameObject> objects;

        public OctreeNode(Bounds bounds, int level, int maxObjectsPerNode, int maxLevels)
        {
            this.bounds = bounds;
            this.level = level;
            this.maxObjectsPerNode = maxObjectsPerNode;
            this.maxLevels = maxLevels;
            children = new OctreeNode[8];
            objects = new List<GameObject>();
        }

        public void Insert(GameObject obj)
        {
            if (children[0] != null)
            {
                int index = GetChildIndex(obj.transform.position);
                if (index != -1)
                {
                    children[index].Insert(obj);
                    return;
                }
            }

            objects.Add(obj);
            // Handle object insertion logic here
        }

        public void Clear()
        {
            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] != null)
                {
                    children[i].Clear();
                    children[i] = null;
                }
            }

            objects.Clear();
            // Clear objects list and handle any necessary cleanup
        }

        public void ActivateObjectsInRange(Vector3 position, float range)
        {
            if (!bounds.Intersects(new Bounds(position, new Vector3(range * 2, range * 2, range * 2))))
                return;

            // Activate or deactivate objects within range and handle any necessary logic

            if (children[0] != null)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    children[i].ActivateObjectsInRange(position, range);
                }
            }
        }

        private int GetChildIndex(Vector3 position)
        {
            // Determine the index of the child node based on the position
            // Return -1 if the position is outside the bounds of the node

            return -1;
        }
    }
}