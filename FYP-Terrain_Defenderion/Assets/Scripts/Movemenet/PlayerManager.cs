using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    CameraManager cameraManager;

    [SerializeField] private bool isInteracting;
    #region isInteracting getter
    public bool getIsInteracting()
    {
        return isInteracting;
    }
    #endregion
    [SerializeField] private bool isUsingRootMotion;
    #region RootMotion getter and setter
    public bool getIsUsingRootMotion()
    {
        return isUsingRootMotion;
    }
    public void setIsUsingRootMotion(bool value)
    {
        isUsingRootMotion = value;
    }
    #endregion
    private void Awake()
    {
        if(animator == null)
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        cameraManager = FindObjectOfType<CameraManager>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();

    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovements();
    }
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();

        isInteracting = animator.GetBool("isInteracting");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        playerLocomotion.setIsJumping(animator.GetBool("isJumping"));
        animator.SetBool("isGrounded", playerLocomotion.getIsGrounded());
    }
}
