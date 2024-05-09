using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
public class BehaviourTracker : MonoBehaviour
{
    
    [SerializeField] private Tracker m_tracker;
    public Tracker TrackerData { get { return m_tracker; } set { m_tracker = value; } }
    public static BehaviourTracker Singleton;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Awake()
    {
        Singleton = this;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class Tracker
    {
        [SerializeField] 
        private List<float> m_trackedPlayerDamage = new List<float>();
        [SerializeField] private List<GameObject> m_trackedDeath = new List<GameObject>();
        [SerializeField] private List<GameObject> m_trackedPlacedStructures = new List<GameObject>();

        #region Tracked Damage Functions
        public void AddTrackedDamage(GameObject target, float damage)
        {
            if(target != null && target.transform.root.gameObject.layer == LayerMask.NameToLayer("Player"))
                m_trackedPlayerDamage.Add(damage);
        }
        public void ResetTrackedDamage()
        {
            m_trackedPlayerDamage.Clear();
        }
        public float GetTotalTrackedDamage()
        {
            float amount = 0;
            foreach(float value in m_trackedPlayerDamage )
            {
                
                    amount+= value;
                
            }
            return amount;
        }
        #endregion

        #region Tracked Death Functions
        public void AddTrackedDeath(GameObject target)
        {
            m_trackedDeath.Add(target);
        }
        public void ResetTrackedDeath()
        {
            m_trackedDeath.Clear();
        }
        public int GetTrackedDeathAmount(LayerMask targetLayer = default)
        {
            int amount = 0;
            foreach(GameObject d in m_trackedDeath)
            {
                if(d.layer == targetLayer || targetLayer == default)
                {
                    amount++;
                }
            }

            return amount;
        }
        #endregion

        #region Tracked Placed Structures Functions
        public void AddTrackedPlacedStructure(GameObject target)
        {
            m_trackedPlacedStructures.Add(target);
        }
        public void ResetTrackedPlacedStructure()
        {
            m_trackedPlacedStructures.Clear();
        }
        public void RemovePlacedStructure(GameObject gameObject)
        {
            m_trackedPlacedStructures.Remove(gameObject);
        }
        public int GetTrackedPlacedStructure()
        {
            int amount = 0;
            foreach (GameObject d in m_trackedPlacedStructures)
            {

                    amount++;
                
            }

            return amount;
        }
        #endregion
    }
}
