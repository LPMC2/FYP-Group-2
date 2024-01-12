using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    InputManager inputManager;
    AnimatorManager animatorManager;

    Vector3 moveDirection;
    Transform cameraObject;
    public Rigidbody playerRigidbody;

    [Header("Falling Settings")]
    [SerializeField] private float inAirTimer;
    [SerializeField] private float leapingVelocity;
    [SerializeField] private float fallingSpeed;
    [SerializeField] private float rayCastHeightOffset = 0.5f;
    [SerializeField] private float fallingCollisionRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    #region AirTimer getter
    public float getInAirTimer()
    {
        return inAirTimer;
    }
    #endregion

    [Header("Movement Flags")]
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    #region isSprinting & isJumping Setter and isGrounded getter
    public void setIsSprinting(bool value)
    {
        isSprinting = value;
    }
    public void setIsJumping(bool value)
    {
        isJumping = value;
    }
    public bool getIsSprinting()
    {
        return isSprinting;
    }
    public bool getIsGrounded()
    {
        return isGrounded;
    }
    #endregion

    [Header("Movement Speed Setting")]
    [SerializeField] private float walkingSpeed = 1.5f;
    [SerializeField] private float movementSpeed = 7f;
    [SerializeField] private float sprintingSpeed = 7;
    [SerializeField] private float rotationSpeed = 15f;

    [Header("Jump Speeds")]
    [SerializeField] private float jumpHeight = 3;
    [SerializeField] private float gravityIntensity = -15;
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerManager = GetComponent<PlayerManager>();
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    private void HandleMovement()
    {
        if(isJumping)
        {
            return;
        }
        moveDirection = cameraObject.forward * inputManager.getVerticalInput();
        moveDirection = moveDirection + cameraObject.right * inputManager.getHorizontalInput();
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(isSprinting)
        {
            moveDirection *= sprintingSpeed;
        }else
        {
            if (inputManager.getMoveAmount() >= 0.5f)
            {
                moveDirection *= movementSpeed;
            }
            else
            {
                moveDirection *= walkingSpeed;
            }
        }

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }
    private void HandleRotation()
    {
        if(isJumping)
        {
            return;
        }
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.getVerticalInput();
        targetDirection = targetDirection + cameraObject.right * inputManager.getHorizontalInput();
        targetDirection.Normalize();
        targetDirection.y = 0;

        if(targetDirection == Vector3.zero)
        {
            targetDirection = transform.forward;
        }
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    public void HandleAllMovements()
    {
        HandleFallingAndLanding();
        if(playerManager.getIsInteracting())
        {
            return;
        }
        HandleMovement();
        HandleRotation();
    }
    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;
        targetPosition = transform.position;

        if(!isGrounded && !isJumping)
        {
            if(!playerManager.getIsInteracting())
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }
            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingSpeed * inAirTimer);
        }

        if(Physics.SphereCast(rayCastOrigin, fallingCollisionRadius, -Vector3.up, out hit, groundLayer))
        {
            //Debug.Log(hit.collider.gameObject + "\n" + rayCastOrigin + -Vector3.up);
            animatorManager.animator.SetBool("isInteracting", false);
            if (!isGrounded)
            {
                
                animatorManager.PlayTargetAnimation("Land", true);
            }
            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        } else
        {
            isGrounded = false;
        }
        if(isGrounded && !isJumping)
        {
            if(playerManager.getIsInteracting() || inputManager.getMoveAmount() > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);

            } else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if(isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }
    public void HandleDodge()
    {
        if(playerManager.getIsInteracting())
        {
            return;
        }
        animatorManager.PlayTargetAnimation("Dodge", true);

    }
}
