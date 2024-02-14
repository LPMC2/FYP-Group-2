using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    PlayerInput playerInputs;

    [SerializeField] private Vector2 movementInput;
    [SerializeField] private float moveAmount;
    [SerializeField] private bool isRightClick;
    [SerializeField] private bool isLeftClick;
    public bool RightMouseClick { get { return isRightClick; } }
    public bool LeftMouseClick { get { return isLeftClick; } }
    PlayerLocomotion playerLocomotion;
    #region moveAmount getter
    public float getMoveAmount()
    {
        return moveAmount;
    }
    #endregion
    [SerializeField] private Vector2 cameraInput;
    [SerializeField] private float cameraInputX;
    [SerializeField] private float cameraInputY;
    #region cameraInput getter
    public float getCameraInputX()
    {
        return cameraInputX;
    }
    public float getCameraInputY()
    {
        return cameraInputY;
    }
    #endregion
    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;
    #region Input Getter
    public float getVerticalInput()
    {
        return verticalInput;
    }
    public float getHorizontalInput()
    {
        return horizontalInput;
    }
    #endregion
    [SerializeField] private bool b_Input;
    [SerializeField] private bool dodge_Input;
    [SerializeField] private bool jump_Input;
    AnimatorManager animatorManager;
    private void Awake()
    {
        animatorManager = GetComponent<AnimatorManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    private void OnEnable()
    {
        if(playerInputs == null)
        {
            playerInputs = new PlayerInput();

            playerInputs.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerInputs.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerInputs.PlayerActions.Sprint.performed += i => b_Input = true;
            playerInputs.PlayerActions.Sprint.canceled += i => b_Input = false;
            playerInputs.PlayerMovement.Jump.performed += i => jump_Input = true;
            playerInputs.PlayerMovement.Dodge.performed += i => dodge_Input = true;
            playerInputs.PlayerActions.Left_Click.performed += i => isLeftClick = true;
            playerInputs.PlayerActions.Left_Click.canceled += i => isLeftClick = false;
            playerInputs.PlayerActions.Right_Click.performed += i => isRightClick = true;
            playerInputs.PlayerActions.Right_Click.canceled += i => isRightClick = false;
        }
        playerInputs.Enable();
    }
    private void OnDisable()
    {
        movementInput = Vector2.zero;
        cameraInput = Vector2.zero;
        moveAmount = 0f;
        jump_Input = false;
        verticalInput = 0f;
        horizontalInput = 0f;
        cameraInputX = 0f;
        cameraInputY = 0f;
        isLeftClick = false;
        isRightClick = false;
        playerInputs.Disable();

    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleSprintingInput();
        HandleJumpingInput();
        HandleDodgeInput();
    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;

        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount, playerLocomotion.getIsSprinting());
    }

    private void HandleSprintingInput()
    {
        if(b_Input && moveAmount > 0.5f)
        {
            playerLocomotion.setIsSprinting(true);
        } else
        {
            playerLocomotion.setIsSprinting(false);
        }
    }

    private void HandleJumpingInput()
    {
        if(jump_Input)
        {
            jump_Input = false;
            playerLocomotion.HandleJumping();
        }
    }
    private void HandleDodgeInput()
    {
        if(dodge_Input)
        {
            dodge_Input = false;
            playerLocomotion.HandleDodge();
        }    
    }
}
