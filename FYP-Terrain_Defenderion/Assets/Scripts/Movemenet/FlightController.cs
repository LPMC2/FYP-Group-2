using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlightController : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float movementSpeed = 5f;
    [SerializeField] float movementSpeedMultiplier = 1f;
    [SerializeField] float rotationSpeed = 3f;
    [SerializeField] private bool isRotatable = true;
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] PlayerInput playerInput;
    bool isMoveable = false;
    InputAction moveAction;
    InputAction lookAction;
    InputAction updownAction;
    private Vector2 cameraInput;
    Vector2 movementInput;
    private Vector3 velocity;
    private Vector2 look;
    private CharacterController controller;
    Vector2 lookInput;
    private void Awake()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();
        }
        moveAction = playerInput.PlayerMovement.Movement;
        lookAction = playerInput.PlayerMovement.Camera;
        updownAction = playerInput.PlayerMovement.FlyUpDown;
    }
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        //playerInput.Enable();
    }
    private void OnEnable()
    {
        //if (playerInput == null)
        //{
        //    playerInput = new PlayerInput();

        //    playerInput.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        //    playerInput.PlayerMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();
        //}
        if(playerInput != null)
        playerInput.Enable();
    }
    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.Disable();
    }
    private void Update()
    {
        if (isMoveable)
        {
            UpdateMovementFlying();
            UpdateLook();
        }
    }
    public void SetMoveable(bool state)
    {
        isMoveable = state;
    }
    private void UpdateLook()
    {
        if(!isRotatable)
        {
            return;
        }
        Vector2 lookInput = lookAction.ReadValue<Vector2>();
        look.x += lookInput.x * mouseSensitivity;
        look.y += lookInput.y * mouseSensitivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }
    Vector3 GetMovementInput(bool horizontal = true)
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        Vector3 input = new Vector3();
        float flyUpDownInput = updownAction.ReadValue<float>();
        Transform referenceTransform = horizontal ? transform : cameraTransform;
        input += referenceTransform.forward * moveInput.y;
        input += referenceTransform.right * moveInput.x;
        if(!horizontal)
        {
            input += transform.up * flyUpDownInput;
        }
        input = Vector3.ClampMagnitude(input, 1f);
        input *= movementSpeed * movementSpeedMultiplier;
        return input;
    }
    void UpdateMovementFlying()
    {
        Vector3 input = GetMovementInput(false);

        float factor = acceleration * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, input, factor);

        controller.Move(velocity * Time.deltaTime);
    }
    public void OnJump()
    {

    }
}