using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDefenseSpawner : MonoBehaviour
{
    [Header("Note: Will only use Default Structures instead")]
    [SerializeField] private StructureManager structureManager;
    [SerializeField] private List<Layer> generateLayers = new List<Layer>();
    [SerializeField] private float offsetY = 0.5f;
    private List<GameObject> structureList = new List<GameObject>();
    public int CalculateBlocksInLayer(int layerNumber)
    {
        if (layerNumber == 0) { return 1; }
        else
            return layerNumber * 8;
    }
    private HashSet<Vector3> placedPositions = new HashSet<Vector3>();
    private Vector3 GetRandomPosition()
    {
        int radius = generateLayers.Count;
        int randomX = Random.Range(-radius, radius + 1);
        int randomZ = Random.Range(-radius, radius + 1);
        Vector3Int randomPosition = new Vector3Int(randomX, 0, randomZ);

        while (placedPositions.Contains(randomPosition))
        {
            randomX = Random.Range(-radius, radius + 1);
            randomZ = Random.Range(-radius, radius + 1);
            randomPosition = new Vector3Int(randomX, 0, randomZ);
        }

        placedPositions.Add(randomPosition);
        return randomPosition;
    }
    private List<SpawnerBehaviour> spawnerBehaviours = new List<SpawnerBehaviour>();
    private void StoreSpawners(GameObject structure)
    {
        foreach (Transform child in structure.transform)
        {
            SpawnerBehaviour spawnerBehaviour = child.GetComponent<SpawnerBehaviour>();
            if (spawnerBehaviour != null)
            {
                spawnerBehaviours.Add(spawnerBehaviour);
                spawnerBehaviour.gameObject.GetComponent<Collider>().enabled = true;
                spawnerBehaviour.gameObject.GetComponent<Collider>().isTrigger = false;
            }
        }
    }
    public void SetSpawnerTeam()
    {
        foreach (SpawnerBehaviour spawnerBehaviour in spawnerBehaviours)
        {
            if (spawnerBehaviour)
            {
                TeamBehaviour.Singleton.TeamManager[1].AddMember(spawnerBehaviour.gameObject);
                spawnerBehaviour.StartSpawn(1);
            }
        }
    }
    public void GenerateDefense()
    {
        GetStructurelist();

        int index = 0;
        //Designed for further development
        foreach (Layer layer in generateLayers)
        {
            if (!layer.IsIgnored)
            {
                int numberOfStructures = CalculateBlocksInLayer(index);
                for (int i = 0; i < numberOfStructures; i++)
                {

                    GameObject targetStructure = null;
                    switch (layer.Type)
                    {
                        case Layer.LayerType.Wall:
                            targetStructure = GetRandomStructure(InteractType.none);
                            break;
                        case Layer.LayerType.Defense:
                            if (Random.Range(0, 100) < 50)
                                targetStructure = GetRandomStructure(InteractType.none);

                            break;
                    }
                    Debug.Log("Structure: " + targetStructure);
                    if (targetStructure != null)
                    {
                        targetStructure.transform.position = gameObject.transform.position + GetRandomPosition() * 15f;
                        targetStructure.layer = LayerMask.NameToLayer("Defense");
                        TeamBehaviour.Singleton.TeamManager[1].AddMember(targetStructure);
                        StoreSpawners(targetStructure);
                        foreach(Transform transform in targetStructure.transform)
                        {
                            if(transform.GetComponent<ShooterManager>() != null)
                            {
                                TeamBehaviour.Singleton.TeamManager[1].AddMember(transform.GetComponent<ShooterManager>().ShooterObj);
                            }
                        }
                        //SetPositionFromLayer(targetStructure, index, i);
                    }

                }
            }
            index++;
        }
    }
    private void SetPositionFromLayer(GameObject target,int layer,int index = 0 )
    {
        int totalBlocks = CalculateBlocksInLayer(layer);

        // Calculate the side based on the index
        int side = index / (totalBlocks / 4);

        // Calculate the position based on the side and index
        Vector3 position = Vector3.zero;

        switch (side)
        {
            case 0: // Top side
                position = new Vector3(0f, offsetY, (index % (totalBlocks / 4)) * 7.5f);
                break;
            case 1: // Right side
                position = new Vector3((index % (totalBlocks / 4)) * 7.5f, offsetY, (totalBlocks / 4) * 7.5f);
                break;
            case 2: // Bottom side
                position = new Vector3(0f, offsetY, (totalBlocks / 4 - (index % (totalBlocks / 4)) - 1) * 7.5f);
                break;
            case 3: // Left side
                position = new Vector3((totalBlocks / 4 - (index % (totalBlocks / 4)) - 1) * 7.5f, offsetY, 0f);
                break;
        }
        target.transform.position = gameObject.transform.position + (position);
    }
    private GameObject GetRandomStructure(InteractType interactType)
    {
        GameObject structure = null;
        int count = 0;
        while (structure == null && count < structureList.Count)
        {
            int randomId = Random.Range(0, structureList.Count);
            GameObject structureTarget = structureList[randomId];
                GridData gridData = structureTarget.GetComponent<GridData>();

                    structure = Instantiate(structureTarget, gameObject.transform.position, Quaternion.identity, gameObject.transform);
                    structure.SetActive(true);
                
            count++;
        }
        return structure;
    }
    private void GetStructurelist()
    {
        foreach (StructurePooling structurePooling in structureManager.structurePoolings)
        {
            if (structurePooling.isDefault)
            {
                structureList.Add(structurePooling.structures[0]);
            }

        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [System.Serializable]
    public class Layer
    {
        [SerializeField] private bool isIgnored = false;
        public bool IsIgnored { get { return isIgnored; } }
        [SerializeField] private LayerType layerType;
        public LayerType Type { get { return layerType; } set { layerType = value; } }
        public enum LayerType
        {
            Default,
            Wall,
            Defense
        }
    }
}
