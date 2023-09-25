using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BodyManager))]
public class CombatBehaviour : MonoBehaviour
{
    [Header("Other Objects/Components")]
    [SerializeField] private CameraManager cameraManager;
    [Header("Attack Settings")]
    [SerializeField] private float damage = 0;
    #region Damage Getter and Setter
    public float getDamage()
    {
        return damage;
    }
    public void setDamage(float value)
    {
        damage = value;
    }
    #endregion
    [Header("Attack Limit")]
    [SerializeField] private Quaternion[] startRotation = new Quaternion[2];
    [SerializeField] private float maxSwingRange = 2f;
    #region Max Swing Range Getter & Setter
    public float getMaxSwingRange()
    {
        return maxSwingRange;
    }
    public void setMaxSwingRange(float value)
    {
        maxSwingRange = value;
    }
    #endregion
    [SerializeField] private float swingCoolDown = 0.5f;
    #region Swing Cooldown Getter & Setter
    public float getSwingCD()
    {
        return swingCoolDown;
    }
    public void setSwingCD(float value)
    {
        swingCoolDown = value;
    }
    #endregion
    [SerializeField] private int maxSwingAmout = 2;
    #region Max Swing Amount Getter & Setter
    public int getMaxSwingAmt()
    {
        return maxSwingAmout;
    }
    public void setMaxSwingAmt(int value)
    {
        maxSwingAmout = value;
    }
    #endregion
    [SerializeField] private float swingSpeed = 1f;
    [SerializeField] private float maxSwingSpeed = 5f;
    #region SwingSpeed Getter & Setter
    public float getSwingSpeed()
    {
        return swingSpeed;
    }
    public void setSwingSpeed(float value)
    {
        swingSpeed = value;
    }
    #endregion
    [SerializeField] private List<int> swingSeq;
    #region Note to swingSeq:
    /*
     * 
     * Value - 0 -> Up, 1 -> down, 2 -> Right, 3 -> Left
     * 
     * 
     */
    #endregion
    [Header("Input")]
    [SerializeField] private InputAction pressed, axis;
    [SerializeField] private GameObject armRig, foreArmRig, handRg;
    private Vector2 rotation;
    private bool rotateAllowed;
    private BodyManager body;
    int preSwingSeq;
    private Animator mainAnimator;
    private void Awake()
    {
        mainAnimator = GetComponent<Animator>();
        body = gameObject.GetComponent<BodyManager>();
        pressed.Enable();
        axis.Enable();
        pressed.performed += _ => { StartCoroutine(Rotate()); };
        pressed.canceled += _ => { rotateAllowed = false;  SetRotateState(true); };
        axis.performed += context => { rotation = context.ReadValue<Vector2>(); }; ;
    }
    private void SetRotateState(bool value)
    {
        if (cameraManager != null)
        {
            cameraManager.SetIsRotateable(value);
        }
    }
    private IEnumerator SwingCD()
    {
        rotateAllowed = false;

        float startTime = Time.time;
        float elapsedTime = 0f;
        float resetTime = 0.5f; // Adjust this value to control the duration of the reset

        Quaternion startRotation1 = armRig.transform.rotation;
        Quaternion startRotation2 = foreArmRig.transform.rotation;
        Quaternion targetRotation1 = startRotation[0];
        Quaternion targetRotation2 = startRotation[1];

        while (elapsedTime < swingCoolDown)
        {
            float t = elapsedTime / swingCoolDown;

            armRig.transform.rotation = Quaternion.RotateTowards(startRotation1, targetRotation1, t * 0.1f);
            foreArmRig.transform.rotation = Quaternion.RotateTowards(startRotation2, targetRotation2, t * 0.1f);

            elapsedTime = Time.time - startTime;
            yield return null;
        }

        // Reset rotation back to target rotation over time
        float resetStartTime = Time.time;
        float resetElapsedTime = 0f;

        while (resetElapsedTime < resetTime)
        {
            float resetT = resetElapsedTime / resetTime;

            armRig.transform.rotation = Quaternion.RotateTowards(targetRotation1, startRotation1, resetT * 0.1f);
            foreArmRig.transform.rotation = Quaternion.RotateTowards(targetRotation2, startRotation2, resetT * 0.1f);

            resetElapsedTime = Time.time - resetStartTime;
            yield return null;
        }

        ResetSwingSequence();
    }
    private IEnumerator Rotate()
    {
        rotateAllowed = true;
        SetRotateState(false);
        while (rotateAllowed)
        {
            int value = GetMouseDirection(rotation);
            if ((Mathf.Abs(rotation.x) >= 5f || Mathf.Abs(rotation.y) >= 5f) && preSwingSeq != value)
            {
                preSwingSeq = value;
                if(swingSeq.Count > 1 && swingSeq[swingSeq.Count-1] == 1 && value == 0)
                {
                    PlayHandAni(4);
                } else
                PlayHandAni(preSwingSeq);
                swingSeq.Add(value);
                if(swingSeq.Count-1 > maxSwingAmout)
                {
                    mainAnimator.enabled = true;
                    SetRotateState(true);
                    StartCoroutine(SwingCD());
                    PlayHandAni(-1);
                    timeCount = 0.0f;
                }
            }
            
            rotation *= swingSpeed;

            rotation.x = Mathf.Clamp(rotation.x, -maxSwingSpeed, maxSwingSpeed);
            rotation.y = Mathf.Clamp(rotation.y, -maxSwingSpeed, maxSwingSpeed);


            swing();
            yield return null;
        }
    }
    private void swing()
    {
        switch (preSwingSeq)
        {
            case 0:
                checkRotationLimit(armRig,new Vector3(-rotation.x, 0f, rotation.y));
                checkRotationLimit(foreArmRig,new Vector3(-rotation.x, 0f, rotation.y));
                break;
            case 1:
                checkRotationLimit(armRig, new Vector3(-rotation.x, 0f, rotation.y));
                checkRotationLimit(foreArmRig, new Vector3(-rotation.x, 0f, rotation.y));
                break;
            case 2:
                checkRotationLimit(armRig, new Vector3(-rotation.x, 0f, 0f));
                checkRotationLimit(foreArmRig, new Vector3(-rotation.x, 0f, -Mathf.Abs(rotation.y)));
                break;
            case 3:
                checkRotationLimit(armRig, new Vector3(-rotation.x, 0f, 0f));
                checkRotationLimit(foreArmRig, new Vector3(-rotation.x, 0f, -Mathf.Abs(rotation.y)));
                break;
        }
        
    }
    private void checkRotationLimit(GameObject rotateObj, Vector3 vecAngle)
    {
        CharacterJoint characterJoint = rotateObj.GetComponent<CharacterJoint>();
        if (characterJoint != null)
        {
            // Get the current rotation
            Vector3 currentRotation = rotateObj.transform.rotation.eulerAngles;
            // Get the minimum and maximum limits of the Character Joint
            Vector3 minTwistLimit = new Vector3(characterJoint.lowTwistLimit.limit, -characterJoint.swing1Limit.limit, -characterJoint.swing2Limit.limit);
            Vector3 maxTwistLimit = new Vector3(characterJoint.highTwistLimit.limit, characterJoint.swing1Limit.limit, characterJoint.swing2Limit.limit);
            minTwistLimit += currentRotation;
            maxTwistLimit += currentRotation;
            // Calculate the new rotation after applying the rotation amount
            Vector3 newRotation = currentRotation + vecAngle;

            // Check if the new rotation is within the limits for all axes
            if (newRotation.x >= minTwistLimit.x && newRotation.x <= maxTwistLimit.x &&
                newRotation.y >= minTwistLimit.y && newRotation.y <= maxTwistLimit.y &&
                newRotation.z >= minTwistLimit.z && newRotation.z <= maxTwistLimit.z)
            {
                rotateObj.transform.Rotate(vecAngle);
                // Rotation is within limits
            }
            else
            {
                // Rotation exceeds limits
            }
        } else
        {
            rotateObj.transform.Rotate(vecAngle);
        }
    }
    public Transform objectToMove;
    Vector2 mouseDirection;
    private bool isHoldingButton = false;
    private float swingCooldown = 1.0f;
    private bool isCooldownActive = false;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        swingSeq = new List<int>();
        StoreStartRotation();
    }
    private void StoreStartRotation()
    {
        startRotation[0] = Quaternion.identity;
        startRotation[1] = Quaternion.identity;
    }
    float speed = 0.01f;
    float timeCount = 0.0f;
    // Update is called once per frame
    private void Update()
    {
    }

    public void PlayHandAni(int pos)
    {
        Animator animator = body.bodyParts.rightArm.hand.handObj.GetComponent<Animator>();
        if (animator != null)
        {
            switch (pos)
            {
                case -1:

                    animator.SetInteger("Pos", -1);
                    break;
                case 0:
                    animator.SetInteger("Pos",0);
                    break;
                case 1:
                    animator.SetInteger("Pos", 1);
                    break;

                case 2:
                    animator.SetInteger("Pos", 2);
                    break;
                case 3:
                    animator.SetInteger("Pos", 3);
                    break;
                case 4:
                    animator.SetInteger("Pos", 4);
                    break;
            }
        }
    }
    
    private int GetMouseDirection(Vector2 direction)
    {
       // Debug.Log(Mathf.Abs(direction.x) + "/" + Mathf.Abs(direction.y));
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            if (direction.x > 0f)
                return 2; // Right side
            else
                return 3; // Left side
        }
        else
        {
            if (direction.y > 0f)
                return 0; // Forward
            else
                return 1; // Backwards
        }
       
    }
    private void ResetSwingSequence()
    {
        if (swingSeq.Count > 0)
        {
            // Clear swing sequence
            swingSeq.Clear();
        }
    }

  

}
