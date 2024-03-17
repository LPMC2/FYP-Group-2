using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryLoadoutUIManager : MonoBehaviour
{
    [SerializeField] private List<Loadout> m_inventoryLoadouts = new List<Loadout>();
    [SerializeField] private GameObject m_slotUIPrefab;
    [SerializeField] private int m_currentLoadout = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [System.Serializable]
    public struct Loadout
    {
        public InventoryLoadoutSO loadoutSO;
        public GameObject DisplayObject;
    }
}
