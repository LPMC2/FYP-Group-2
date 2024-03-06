using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoStoringBehaviour : MonoBehaviour
{
    private InventoryBehaviour inventory;
    [SerializeField]
    private List<AmmoStoring> ammoStoringSystem = new List<AmmoStoring>();
    public List<AmmoStoring> AmmoStoringSystem { get { return ammoStoringSystem; } }
    private void Start()
    {
        inventory = gameObject.GetComponent<InventoryBehaviour>();
    }
    public void StoreAmmo(int id ,int remainAmmo, int totalAmmo)
    {
        Debug.Log("Gun ID: " + id);
        foreach (AmmoStoring ammoStoring in ammoStoringSystem)
        {
            if (ammoStoring.ID == id)
            {
                ammoStoring.RemainAmmo = remainAmmo;
                ammoStoring.TotalAmmo = totalAmmo;
            }
        }

    }
    public int GetRemainAmmo(int id)
    {
        return ammoStoringSystem[id].RemainAmmo;
    }
    public int GetTotalAmmo(int id)
    {
        return ammoStoringSystem[id].TotalAmmo;
    }
    public void InventorySetup()
    {
        if(inventory == null) { inventory = gameObject.GetComponent<InventoryBehaviour>(); }
        Debug.Log("testSetup");
        int count = 0;
        foreach(InventorySystem.Slot slot in inventory.inventory.slot)
        {
            if (slot.getId() != -1)
            {
                Debug.Log(ItemManager.ItemData.item[slot.getId()].model);
                GunController gunController = ItemManager.ItemData.item[slot.getId()].model.GetComponent<GunController>();
                if (gunController != null)
                {
                    AddAmmoStore(gunController, count);
                }
            }
            count++;
        }
    } 
    public void SlotDetection(GameObject target)
    {
        foreach (AmmoStoring ammoStoring in ammoStoringSystem)
        {
            if (ammoStoring.ID == inventory.SelectedSlot)
            {
                if (ItemManager.ItemData.item[inventory.inventory.slot[inventory.SelectedSlot].getId()].model.GetComponent<GunController>() != null)
                {
                    GunController gunController = target.GetComponent<GunController>();
                    if (gunController != null)
                    {
                        gunController.SetRemainAmmo(ammoStoring.RemainAmmo);
                        gunController.SetTotalAmmo(ammoStoring.TotalAmmo);
                        gunController.AmmoStoringSystemId = ammoStoring.ID;
                    }
                }
            }
        }
    }
    public void SetAmmoStorage(int id, GunController gunController)
    {
        gunController.SetRemainAmmo(ammoStoringSystem[id].RemainAmmo);
        gunController.SetTotalAmmo(ammoStoringSystem[id].TotalAmmo);
    }
    public void AddAmmoStore(GunController gunController, int id = -1)
    {
        //foreach(AmmoStoring ammoStoring in ammoStoringSystem)
        //{
        //    if (gunController.AmmoStoringSystemId == ammoStoring.ID) return;
        //}
        
        ammoStoringSystem.Add(new AmmoStoring(gunController.GetRemainAmmo(), gunController.GetTotalAmmo(), id));
        gunController.AmmoStoringSystemId = id;
    }
    public void RemoveAmmoStore(int id)
    {
        ammoStoringSystem.Remove(ammoStoringSystem[id]);
    }
    [Serializable]
    public class AmmoStoring
    {
        [SerializeField] private int m_RemainAmmo;
        [SerializeField] private int m_TotalAmmo;
        [SerializeField] private int id = -1;
        public AmmoStoring(int remainAmmo, int totalAmmo, int id = -1)
        {
            m_RemainAmmo = remainAmmo;
            m_TotalAmmo = totalAmmo;
            this.id = id;
        }
        public int RemainAmmo { get {return m_RemainAmmo; } set { m_RemainAmmo = value; } }
        public int TotalAmmo { get { return m_TotalAmmo; } set { m_TotalAmmo = value; } }
        public int ID { get { return id; } set { id = value; } }
    }
}
