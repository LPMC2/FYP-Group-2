using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class MeleeController : MonoBehaviour
{

    //Obsolete
    /*
    [Header("Attack Settings")]
    [SerializeField] private AttackMethod attackMethod;
    [SerializeField] private bool areaAttack = true;
    [SerializeField] private float AttackSpeed;
    [SerializeField] private float StartHitTime = 0.5f;
    [SerializeField] private float hitboxSizeX = 1f;
    [SerializeField] private float hitboxSizeY = 1f;
    [SerializeField] private float maxDistance = 1f;
    [Header("Customization Settings")]
    [SerializeField] private UnityEvent AttackFunction;
    [SerializeField] private UnityEvent<GameObject> HitFunction;
    */
    private GameObject Player;
    [SerializeField] private AudioClip AttackSound;
    private AudioSource audioSource;
    [Header("Debug")]
    [SerializeField] private bool debugMode = false;
    public enum AttackModeType
    {
        Sword,
        Knife,
        HeavyStrike
    }
    [Header("Animation Settings")]
    [SerializeField] private AnimationBehaviour animationBehaviour;
    //[SerializeField] private int animationLayer = 1;
    //[SerializeField] private string[] playAttackAnimation;
    //private int currentAttackAnimation = 0;
    public GameObject Weapon;
    private WeaponFeature.MeleeData data;
    public WeaponBehaviour weaponBehaviour { get; set; }
    [SerializeField] private AttackModeType AttackMode;

    private bool isAttack = false;
    [Header("Other Settings")]
    [SerializeField] private float SpeedMultiplier = 1f;
    public void SetData(WeaponFeature.MeleeData data)
    {
        this.data = data;
    }
    private void Start()
    {

        Player = gameObject.transform.root.gameObject;
        audioSource = GetComponent<AudioSource>();
        weaponBehaviour = GetComponent<WeaponBehaviour>();
        data = weaponBehaviour.MeleeData;

    }
    public void AttackWeapon()
    {
            attack = StartCoroutine(Attack());

    }
    private void OnDisable()
    {
        ResetAttack();
    }
    private void ResetAttack()
    {
        if(debugMode)
        {
            Debug.Log("Resetted!");
        }
        if(attack != null)
        StopCoroutine(attack);
        if(attacking != null)
        StopCoroutine(attacking);
        if(attackAnimation != null)
        StopCoroutine(attackAnimation);
        hitObjects.Clear();
        isAttack = false;
    }
    private Coroutine attack;
    private Coroutine attacking;
    private Coroutine attackAnimation;
    private IEnumerator Attack()
    {
        if (audioSource != null)
        {
            audioSource.clip = AttackSound;
            audioSource.Play();
        }
        //isAttack = true;
        //attackAnimation = StartCoroutine(AttackAnim());
        yield return new WaitForSeconds(weaponBehaviour.useCD* data.startHitTime);
        switch (data.AttackMethod)
        {
            case AttackMethod.HitBox:
                hitbox();
                break;
            case AttackMethod.Collision:
                attacking = StartCoroutine(StartAttack());
                break;
        }
        
        /*yield return new WaitForSeconds(AttackSpeed* (1- StartHitTime));
        isAttack = false;
        if (audioSource != null)
        {
            audioSource.clip = null;
        }
        */
    }
    private HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    private void hitbox()
    {
        //AttackFunction.Invoke();
        // Calculate the area of effect
        float halfWidth = data.hitBoxX / 2f;
        float halfHeight = data.hitBoxY / 2f;
        Vector3 origin = Camera.main.transform.position + Camera.main.transform.forward;
        Vector3 corner1 = origin + Camera.main.transform.right * halfWidth + Camera.main.transform.up * halfHeight;
        Vector3 corner2 = origin - Camera.main.transform.right * halfWidth - Camera.main.transform.up * halfHeight;

        // Cast the boxcast
        Vector3 size = new Vector3(halfWidth, halfHeight, data.AtkDis);
        RaycastHit[] hits = Physics.BoxCastAll(origin, size, Camera.main.transform.forward * -1f, Quaternion.identity, data.AtkDis, weaponBehaviour.affectedLayers);
        if (debugMode)
        {
            Vector3[] corners = new Vector3[8];
            corners[0] = origin + Camera.main.transform.right * halfWidth + Camera.main.transform.up * halfHeight;
            corners[1] = origin + Camera.main.transform.right * halfWidth - Camera.main.transform.up * halfHeight;
            corners[2] = origin - Camera.main.transform.right * halfWidth - Camera.main.transform.up * halfHeight;
            corners[3] = origin - Camera.main.transform.right * halfWidth + Camera.main.transform.up * halfHeight;
            corners[4] = corners[0] + Camera.main.transform.forward * data.AtkDis;
            corners[5] = corners[1] + Camera.main.transform.forward * data.AtkDis;
            corners[6] = corners[2] + Camera.main.transform.forward * data.AtkDis;
            corners[7] = corners[3] + Camera.main.transform.forward * data.AtkDis;
            Debug.DrawRay(corners[0], corners[1] - corners[0], Color.red, 2f);
            Debug.DrawRay(corners[1], corners[2] - corners[1], Color.red, 2f);
            Debug.DrawRay(corners[2], corners[3] - corners[2], Color.red, 2f);
            Debug.DrawRay(corners[3], corners[0] - corners[3], Color.red, 2f);
            Debug.DrawRay(corners[4], corners[5] - corners[4], Color.red, 2f);
            Debug.DrawRay(corners[5], corners[6] - corners[5], Color.red, 2f);
            Debug.DrawRay(corners[6], corners[7] - corners[6], Color.red, 2f);
            Debug.DrawRay(corners[7], corners[4] - corners[7], Color.red, 2f);
            Debug.DrawRay(corners[0], corners[4] - corners[0], Color.red, 2f);
            Debug.DrawRay(corners[1], corners[5] - corners[1], Color.red, 2f);
            Debug.DrawRay(corners[2], corners[6] - corners[2], Color.red, 2f);
            Debug.DrawRay(corners[3], corners[7] - corners[3], Color.red, 2f);
        }
        foreach (RaycastHit hit in hits)
        {

            if (!hitObjects.Contains(hit.collider.gameObject)) // Check if the object has already been hit
            {
                //HitFunction.Invoke(hit.collider.gameObject);
                hitObjects.Add(hit.collider.gameObject); // Add the object to the HashSet
                HitDamage(hit.collider.gameObject);
                if (!data.isAOE)
                {
                    hitObjects.Clear();
                    return;
                }

            }


        }

        hitObjects.Clear();
    }
    private IEnumerator StartAttack()
    {
        if(debugMode)
        Debug.Log("StartAttack");
        CollisionDetector collisionDetector = gameObject.GetComponent<CollisionDetector>();
        collisionDetector.IncludedLayerMask = weaponBehaviour.affectedLayers;
        float attacktimer = 0f;
        while(attacktimer < weaponBehaviour.useCD * data.startHitTime)
        {
            attacktimer += Time.deltaTime;
            if(collisionDetector != null && collisionDetector.isHit && collisionDetector.HitEntity != null)
            {
                if (!hitObjects.Contains(collisionDetector.HitEntity)) // Check if the object has already been hit
                {
                    if(debugMode)
                    Debug.Log("HitEntity");
                    //HitFunction.Invoke(collisionDetector.HitEntity);
                    hitObjects.Add(collisionDetector.HitEntity); // Add the object to the HashSet
                    HitDamage(collisionDetector.HitEntity);
                    if (!data.isAOE)
                    {
                        hitObjects.Clear();
                        attacktimer = weaponBehaviour.useCD * data.startHitTime;
                    }

                }
            }
            yield return null;
        }
        hitObjects.Clear();
    }
    private void HitDamage(GameObject hit)
    {
        weaponBehaviour.HitTakeDamage(hit);
    }
    private IEnumerator AttackAnim()
    {
        int test = arrayBehaviour.GetRandomObjectFromList(data.AtkAniID);
        if (weaponBehaviour != null)
        weaponBehaviour.PlayAnimation(arrayBehaviour.GetRandomObjectFromList(data.AtkAniID), weaponBehaviour.useCD);
            //animationBehaviour.StartAnimationRandom(weaponAnimator, weaponBehaviour.useCD);
                //if (playAttackAnimation.Length > currentAttackAnimation || playAttackAnimation.Length == 1)
                //{
                //    float speedMultiplier = (1.0f / AttackSpeed);
                //    weaponAnimator.SetFloat("SpeedMultiplier", speedMultiplier);
                //    weaponAnimator.CrossFade(playAttackAnimation[currentAttackAnimation], 0.2f, animationLayer);
                //    currentAttackAnimation = Random.Range(0, playAttackAnimation.Length);
                //    if (currentAttackAnimation >= playAttackAnimation.Length) currentAttackAnimation = 0;
                //}

                //switch (AttackMode)
                //{
                //    case AttackModeType.Sword:
                //        weaponAnimator.Play("SwordSwing");
                //        break;
                //    case AttackModeType.Knife:
                //        weaponAnimator.Play("KnifeSwing");
                //        break;
                //    case AttackModeType.HeavyStrike:
                //        weaponAnimator.Play("HeavySwing");
                //        break;
                //    default:
                //        Debug.LogWarning("Invalid AttackMode: " + AttackMode);
                //        break;
                //}

                yield return null;


                //yield return new WaitForSeconds(weaponAnimator.GetCurrentAnimatorStateInfo(0).length / speedMultiplier);
                //weaponAnimator.Play("Idle");
            
        
    }


}
public enum AttackMethod
{
    HitBox,
    Collision
}