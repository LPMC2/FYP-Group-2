using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AmmoBehaviour : MonoBehaviour
{
    [SerializeField]
    private WeaponBehaviour weaponBehaviour;
    InventoryBehaviour Inventory;
    AmmoStoringBehaviour ammoStoringBehaviour;
    InputAction reloadAction;
    Coroutine reloadCoroutine;
    public WeaponFeature.AmmoData AmmoData { get { return weaponBehaviour.AmmoData; } }
    public void Start()
    {
        if(weaponBehaviour == null)
        {
            weaponBehaviour = GetComponent<WeaponBehaviour>();
        }


        Inventory = transform.root.GetComponent<InventoryBehaviour>();
        ammoStoringBehaviour = transform.root.GetComponent<AmmoStoringBehaviour>();
        if(AmmoData.ReloadInputActionReference == null)
        {
            AmmoData.ReloadInputActionReference = InputActionReference.Create(new PlayerInput().PlayerActions.Reload);
          
        }
        reloadAction = AmmoData.ReloadInputActionReference.ToInputAction();
        if((weaponBehaviour.Features & WeaponFeature.WeaponFeatures.ANIMATIONS ) != 0)
        {
            AmmoData.OnReloadEvent.AddListener(() =>
            {
                weaponBehaviour.PlayAnimation(AmmoData.AnimationIDReload, reloadTime);
            });
        }
    }
    private void OnEnable()
    {
        if (weaponBehaviour == null)
        {
            weaponBehaviour = GetComponent<WeaponBehaviour>();
        }
        if (AmmoData.ReloadInputActionReference == null)
        {
            AmmoData.ReloadInputActionReference = InputActionReference.Create(new PlayerInput().PlayerActions.Reload);

        }
        if(reloadAction == null)
            reloadAction = AmmoData.ReloadInputActionReference.ToInputAction();
        reloadAction.performed += i => { ReloadAmmo(); };
        reloadAction.Enable();
    }
    private void OnDisable()
    {
        reloadAction.performed -= i => { ReloadAmmo(); };
        reloadAction.Disable();
    }
    private void ReloadAmmo()
    {
        if (AmmoData.RemainAmmo > AmmoData.AmmoCount || AmmoData.TotalAmmo <= 0 || !weaponBehaviour.IsActive) return;
        if (reloadCoroutine == null)
        {
            reloadCoroutine = StartCoroutine(ReloadCoroutine());
        }
    }
    private void OnDestroy()
    {
        if(reloadCoroutine != null)
        {
            StopCoroutine(reloadCoroutine);
            reloadCoroutine = null;
        }
    }
    public void AddAmmo(int count)
    {
        AmmoData.TotalAmmo+= count;
        StoreAmmoData();
        UpdateInv();

    }
    public void UseAmmo()
    {
        AmmoData.RemainAmmo--;
        StoreAmmoData();
        UpdateInv();
        if(AmmoData.TotalAmmo <= 0 && AmmoData.RemainAmmo <= 0)
        {
            weaponBehaviour.IsActive = false;
        }
    }
    private float reloadTime = 0f;
    private IEnumerator ReloadCoroutine()
    {
        weaponBehaviour.IsActive = false;
        reloadTime = AmmoData.ReloadTime;
        Inventory.UpdateSlotDisplay("Reloading...");
        AmmoData.OnReloadEvent?.Invoke();
        yield return (!weaponBehaviour.isCD);
        while (reloadTime > 0)
        {
            reloadTime -= Time.deltaTime;
            yield return null;
        }
        StopCoroutine(reloadCoroutine);
        reloadCoroutine = null;

        //Reload Complete Function
        int StoreRemain = AmmoData.RemainAmmo;

        if (AmmoData.TotalAmmo >= AmmoData.AmmoCount)
        {
            AmmoData.RemainAmmo = AmmoData.AmmoCount;
        }
        else if (AmmoData.TotalAmmo < AmmoData.AmmoCount && (AmmoData.TotalAmmo + StoreRemain <= AmmoData.AmmoCount))
        {
            AmmoData.RemainAmmo = AmmoData.TotalAmmo + StoreRemain;

        }
        else
        {
            AmmoData.RemainAmmo = AmmoData.AmmoCount;
        }
        AmmoData.TotalAmmo -= (AmmoData.AmmoCount - StoreRemain);
        if (AmmoData.TotalAmmo < 0)
        {
            AmmoData.TotalAmmo = 0;
        }
        //Update Data
        UpdateInv();
        StoreAmmoData();
        weaponBehaviour.IsActive = true;
    }
    private void StoreAmmoData()
    {
        ammoStoringBehaviour.StoreAmmo(AmmoData.AmmoStoringSystemId, AmmoData.RemainAmmo, AmmoData.TotalAmmo);
    }
    private void Update()
    {
        if(weaponBehaviour != null)
        {
            if ((AmmoData.RemainAmmo <= 0 && AmmoData.TotalAmmo != 0 ) && reloadCoroutine == null)
            {
                
                ReloadAmmo();
            }
        }
    }

    public void SetBehaviour(WeaponBehaviour behaviour)
    {
        weaponBehaviour = behaviour;
    }

    public void UpdateInv()
    {
        if (Inventory != null)
        {
            Inventory.UpdateSlotDisplay(weaponBehaviour.AmmoData.RemainAmmo + "/" + weaponBehaviour.AmmoData.TotalAmmo);
        }
    }

}
