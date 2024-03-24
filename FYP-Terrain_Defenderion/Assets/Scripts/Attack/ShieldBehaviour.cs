using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehaviour : MonoBehaviour
{
    [SerializeField] private WeaponBehaviour weaponBehaviour;
    HealthBehaviour healthBehaviour;
    // Start is called before the first frame update
    void Start()
    {
        if(weaponBehaviour == null)
        {
            weaponBehaviour = GetComponent<WeaponBehaviour>();
        }
        healthBehaviour = weaponBehaviour.owner.GetComponent<HealthBehaviour>();
        if (healthBehaviour != null)
        {
            healthBehaviour.OnHitEvent.AddListener(OnHit);
        }

    }
    public void OnEnable()
    {

    }
    public void OnDestroy()
    {
        if (healthBehaviour != null)
        {
            healthBehaviour.OnHitEvent.RemoveListener(OnHit);
            SetShieldUse(false);
        }
    }
    public void SetShieldUse(bool state)
    {
        healthBehaviour.DeezNuts(state);
    }
    public void OnHit()
    {
        Debug.Log("HIT!");
        SetShieldUse(false);
        weaponBehaviour.StartActiveCoroutine();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
