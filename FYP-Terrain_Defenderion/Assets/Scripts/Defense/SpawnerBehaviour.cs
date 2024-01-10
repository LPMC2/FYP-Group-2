using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class MonsterData
{
    public GameObject Monster;
    public float SpawnChance;

    [HideInInspector]
    public string SpawnChanceString;

    public void UpdateSpawnChanceString()
    {
        SpawnChanceString = "%";
    }
    private ObjectPool objectPooling = new ObjectPool();
    public ObjectPool objectPool { get { return objectPooling; } set { objectPooling = value; } }
   
}


    [System.Serializable]
public class SpawnerBehaviour : MonoBehaviour
{
    private List<int> IdList = new List<int>();
    [SerializeField] private bool SpawnOnStart = false;
    [SerializeField] private int defaultTeamId = -1;
    [SerializeField] private List<MonsterData> monsters = new List<MonsterData>();
    [SerializeField] private LayerMask m_LayerToPlace;
    [SerializeField] private float m_SpawnRadius = 5f;
    [SerializeField] private float minSpawnTime = 15f;
    [SerializeField] private float maxSpawnTime = 20f;
    [SerializeField] private int SpawnCountLimit = 10;
    [SerializeField] private int SpawnAmout = 1;
    [SerializeField] private GameObject TargetObject;
    [SerializeField] private float StayTime = 0f;
    [SerializeField] private int SpawnCounter = 0;
    private int CurrentCounter;
    public List<MonsterData> Monsters
    {
        get { return monsters; }
        set { monsters = value; }
    }
    public bool isMaxMobSpawned()
    {
        if (monsters.Count == 0 || monsters == null) return false;
        int count = 0;
        foreach (MonsterData monsterData in monsters) {
                foreach (GameObject gameObject in monsterData.objectPool.ObjectPooling)
                {
                    if (gameObject.activeInHierarchy)
                    {
                        count++;
                        if (count >= SpawnCountLimit) return true;
                    }
                }

            
        }
        return false;

    }
    public virtual void SetTeam(int id)
    {
        TeamBehaviour teamBehaviour = TeamBehaviour.Singleton;
        defaultTeamId = id;
        teamBehaviour.TeamManager[id].AddMember(gameObject);
    }
    private void SetSpawnedMember(GameObject target)
    {
        TeamBehaviour teamBehaviour = TeamBehaviour.Singleton;
        teamBehaviour.TeamManager[defaultTeamId].AddMember(target);
    }
    public void AddMonster(int index)
    {
        monsters.Insert(index, null);
    }
    public void RemoveMonster(int index)
    {
        monsters.RemoveAt(index);
    }
    // Start is called before the first frame update
    void Start()
    {
        if(defaultTeamId != -1)
        {
            SetTeam(defaultTeamId);
        }
        Initialize();
        SpawnCounter = 0;
        if(SpawnOnStart)
            InvokeRepeating("Spawn", 0f, Random.Range(minSpawnTime, maxSpawnTime));
        
    }
    private void Initialize()
    {
        GameObject initialBase = new GameObject("Object Pool - Base");
        initialBase.transform.SetParent(transform);
        foreach(MonsterData monsterData in monsters)
        {
            GameObject baseObj = monsterData.objectPool.Initialize(monsterData.Monster, SpawnCountLimit);
            baseObj.transform.SetParent(initialBase.transform);
        }
    }
    public void startSpawn()
    {
        InvokeRepeating("Spawn", 0f, Random.Range(minSpawnTime, maxSpawnTime));
    }
    public void setMinSpSpeed(float min)
    {
        minSpawnTime = min;
    }
    public void setMaxSpSpeed(float max)
    {
        maxSpawnTime = max;
    }
    public void setStayTime(float time)
    {
        StayTime = time;
    }
    public void setSpawnCount(float count)
    {
        SpawnAmout = (int)count;
    }
    public void rmSpawnCounter()
    {
        SpawnCounter--;
        if (SpawnCounter < 0) { SpawnCounter = 0; }
    }
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // List to keep track of spawned monsters
    void Spawn()
    {
        if (isMaxMobSpawned())
        {
            return;
        }
        for (int i = 0; i < SpawnAmout; i++)
        {
            bool isSpawned = false;
            
            while (!isSpawned)
            {
                int count = 0;
                foreach (MonsterData monsterData in monsters)
                {
                    if (Random.Range(0f, 100f) <= monsterData.SpawnChance)
                    {
                        GameObject newObject = monsterData.objectPool.GetObject(true);
                        if (newObject == null) {
                            continue; 
                        }
                        Vector2 randomPoint = Random.insideUnitCircle.normalized * Random.Range(-m_SpawnRadius, m_SpawnRadius);
                        Vector3 spawnPosition = transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);
                        newObject.transform.position = spawnPosition;
                        if(defaultTeamId != -1)
                        {
                            SetSpawnedMember(newObject);
                        }
                        //Collider[] colliders = Physics.OverlapSphere(transform.position, m_SpawnRadius, m_LayerToPlace);

                        //if (colliders.Length > 0)
                        //{
                        //    int randomIndex = Random.Range(0, colliders.Length);
                        //    Collider targetCollider = colliders[randomIndex];

                        //    Vector3 spawnPosition = GetRandomPositionOnCollider(newObject, targetCollider);
                        //    newObject.transform.position =  spawnPosition;
                        //}
                        if (newObject.GetComponent<Collider>() != null)
                        {
                            Physics.IgnoreCollision(newObject.GetComponent<Collider>(), GetComponent<Collider>());
                        }

                        if (StayTime > 0)
                        {
                            GameObjectExtension.DisableFromTime(this, newObject, StayTime);
                        }

                        isSpawned = true;
                        break;
                    }
                    count++;
                }
               
            }
        }

    }
    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 0.1f); // Use a small radius for accuracy

        return colliders.Length > 0;
    }
    private Vector3 GetRandomPositionOnCollider(GameObject targetSpawnObj, Collider collider)
    {
        Vector3 colliderCenter = collider.bounds.center;
        Vector3 colliderExtents = collider.bounds.extents;

        float randomX = Random.Range(colliderCenter.x - colliderExtents.x, colliderCenter.x + colliderExtents.x);
        float randomZ = Random.Range(colliderCenter.z - colliderExtents.z, colliderCenter.z + colliderExtents.z);
        float spawnY = colliderCenter.y + colliderExtents.y + targetSpawnObj.transform.localScale.y;

        return new Vector3(randomX, spawnY, randomZ);
    }
    private IEnumerator FadeOut(GameObject obj, float duration, float stayTime)
    {
        // Get the object's material and store its initial color
        Renderer renderer = obj.GetComponent<Renderer>();
        Color initialColor = renderer.material.color;

        // Wait for the specified stay time before starting to fade out the object
        yield return new WaitForSeconds(stayTime);

        // Gradually decrease the object's alpha value over time
        float startTime = Time.time;
        float endTime = startTime + duration;
        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / duration;
            Color newColor = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(initialColor.a, 0f, t));
            renderer.material.color = newColor;
            yield return null;
        }

        // Destroy the object once it has completely faded out
        Destroy(obj);
    }

}
[System.Serializable]
public class ObjectPool
{
    private List<GameObject> objects = new List<GameObject>();
    public virtual List<GameObject> ObjectPooling { get { return objects; } set { objects = value; } }
    public virtual GameObject GetObject(bool activeObject = false, Vector3 position = default, Quaternion rotation = default)
    {
        if(position == default) { position = Vector3.zero; }
        if(rotation == default) { rotation = Quaternion.identity; }
        foreach(GameObject gameObject in objects)
        {
            if(!gameObject.activeInHierarchy)
            {
                if (activeObject)
                {
                    gameObject.SetActive(true);
                    gameObject.transform.position = position;
                    gameObject.transform.rotation = rotation;
                }
                return gameObject;
            }
        }
        return null;
    }
    public GameObject Initialize(GameObject targetObject, int count)
    {
        GameObject basePool = new GameObject("Base Pool: " + targetObject.name);
        string debug = "";
        for(int i=0; i<count; i++)
        {
            GameObject spawnedObj = Object.Instantiate(targetObject, Vector3.zero, Quaternion.identity);
            spawnedObj.transform.SetParent(basePool.transform);
            spawnedObj.name += " - Id: " + i+1;
            debug += spawnedObj.name + "\n";
            objects.Add(spawnedObj);
        }
        return basePool;
    }

}