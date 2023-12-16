using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class HealthBehaviour : MonoBehaviour, IDamageable
{
    [SerializeField] private HealthBarType m_healthBarType;
    [SerializeField] private bool isDynamicHealthBar = false;
    [SerializeField] private float health = 100f;
    [SerializeField] private float hitTime = 0.1f;
    private float initialHealth;
    private float damage = 0;
    public GameObject healthBarPrefab;
    [SerializeField] private Vector3 healthbarOffset;
    [SerializeField] private float DeathTime = 0f;
    [SerializeField]
    private GameObject healthBarUI;
    private Slider healthBarSlider;
    private float lerpSpeed = 0.1f;
    private float lerpTimer;
    [SerializeField] private float chipSpeed = 2f;
    [SerializeField] private AudioClip HurtSound;
    [SerializeField] private bool Invincible = false;
    private bool isHit = false;
    public Image frontHealthBar;
    public Image backHealthBar;
    private Animator MobAnimator;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        initialHealth = health;
        audioSource = GetComponent<AudioSource>();
    }

    public void SetDamage(float damageAmount)
    {
        damage = damageAmount;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetInitialHealth()
    {
        return initialHealth;
    }
    public void SetHealth(float healthAMT)
    {
        health = healthAMT;
        health = Mathf.Clamp(health, 0, initialHealth);
    }
    public void AddHealth(float healthAmount)
    {
        health += healthAmount;
        health = Mathf.Clamp(health, 0, initialHealth);
        if (health >= initialHealth)
        {
            health = initialHealth;
        }
        if (m_healthBarType != HealthBarType.None)
        {
            UpdateHealthBar();
            lerpTimer = 0f;
        }
    }
    private IEnumerator HitEnumerator()
    {
        isHit = true;
        yield return new WaitForSeconds(hitTime);
        isHit = false;
    }
    public void TakeDamage(float damage)
    {
        if (isHit) return;
        if (audioSource != null)
        {
            audioSource.PlayOneShot(HurtSound, 1);
        }
        if (Invincible == false)
        {
           
            health -= damage;
            health = Mathf.Clamp(health, 0, initialHealth);
            if (healthBarPrefab != null)
            {
                lerpTimer = 0;
                UpdateHealthBar();
            }

            if (health <= 0)
            {
               

                
                    if (DeathTime > 0)
                    {
                       if(gameObject.GetComponent<Collider>() != null)
                    {
                        Destroy(gameObject.GetComponent<Collider>());
                    }
                     
                            Destroy(gameObject, DeathTime);
                    }
                        else
                        {
                            Destroy(gameObject);
                        }
                 

                
            }
        
        }
        StartCoroutine(HitEnumerator());
    }

    private void UpdateHealthBar()
    {
        if (m_healthBarType == HealthBarType.WorldSpace)
        {
            
            if (healthBarUI == null)
            {
                if (healthBarPrefab == null) return;
                healthBarUI = Instantiate(healthBarPrefab, transform.position, Quaternion.identity) as GameObject;
                healthBarUI.transform.SetParent(transform);
                healthBarUI.transform.localPosition = new Vector3(healthbarOffset.x, healthbarOffset.y, healthbarOffset.z); // Set the position to be above the enemy's head
                healthBarSlider = healthBarUI.transform.GetChild(0).GetComponent<Slider>();
                if (!isDynamicHealthBar)
                {
                    healthBarSlider.maxValue = initialHealth;
                    healthBarSlider.value = health;
                }
                else
                {
                    frontHealthBar = GameObjectExtension.GetGameObjectWithTagFromChilds(healthBarUI, "FrontHP").GetComponent<Image>();
                    backHealthBar = GameObjectExtension.GetGameObjectWithTagFromChilds(healthBarUI, "BackHP").GetComponent<Image>();
                }
                healthBarUI.GetComponent<Canvas>().worldCamera = Camera.main;
            }
            else
            {
                if(!isDynamicHealthBar)
                healthBarSlider.value = health;
            }

        }

    }
    float currentHealth = 0f;
    // Update is called once per frame
    void Update()
    {
        if (m_healthBarType != HealthBarType.None)
        {
            if (currentHealth != health)
            {
                lerpTimer = 0f;
                currentHealth = health;
            }
            UpdateHealthUI();
        }
        if (healthBarUI != null)
        {

            if (m_healthBarType == HealthBarType.WorldSpace)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0f; // Set the y-component to 0 to remove rotation along the y-axis

                healthBarUI.transform.LookAt(healthBarUI.transform.position + cameraForward,Vector3.up);
            }
        }

    }
    private void UpdateHealthUI()
    {
        if (frontHealthBar == null || backHealthBar == null) return;
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / initialHealth;
        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = new Color(0.5f, 0, 0, 1);
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if(fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

}
public enum HealthBarType
{
    None,
    CameraUI,
    WorldSpace
}