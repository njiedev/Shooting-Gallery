using UnityEngine;
using UnityEngine.InputSystem;

// Stands in for the Standard Assets FirstPersonController from Firstperson.unitypackage.
// That script reads the legacy Input Manager (CrossPlatformInput), which this project has disabled
// in favour of the Input System, so the same walk/run/jump/mouse-look behaviour is reproduced here.
[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    Camera firstPersonCamera;

    [SerializeField]
    float walkSpeed = 5f;

    [SerializeField]
    float runSpeed = 10f;

    [SerializeField]
    float jumpSpeed = 10f;

    [SerializeField]
    float stickToGroundForce = 10f;

    [SerializeField]
    float gravityMultiplier = 2f;

    // Degrees per pixel of mouse movement (legacy "Mouse X" axis scale of 0.1 times the package's sensitivity of 2).
    [SerializeField]
    float mouseSensitivity = 0.2f;

    [SerializeField]
    bool lockCursor = true;

    CharacterController controller;
    float verticalVelocity = 0;
    float pitch = 0;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        UpdateCursorLock(keyboard, mouse);
        Look(mouse);
        Move(keyboard);
    }

    void UpdateCursorLock(Keyboard keyboard, Mouse mouse)
    {
        if (!lockCursor)
        {
            return;
        }

        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else if (mouse != null && mouse.leftButton.wasPressedThisFrame)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Look(Mouse mouse)
    {
        if (mouse == null || (lockCursor && Cursor.lockState != CursorLockMode.Locked))
        {
            return;
        }

        // The first frame after locking the cursor can report a large jump.
        Vector2 delta = Vector2.ClampMagnitude(mouse.delta.ReadValue(), 200f) * mouseSensitivity;

        transform.Rotate(0, delta.x, 0);
        pitch = Mathf.Clamp(pitch - delta.y, -90f, 90f);
        firstPersonCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    void Move(Keyboard keyboard)
    {
        float strafe = 0;
        float forward = 0;
        bool running = false;
        bool jump = false;

        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) strafe -= 1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) strafe += 1;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) forward -= 1;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) forward += 1;

            running = keyboard.leftShiftKey.isPressed;
            jump = keyboard.spaceKey.wasPressedThisFrame;
        }

        Vector3 move = transform.right * strafe + transform.forward * forward;
        move = Vector3.ClampMagnitude(move, 1f) * (running ? runSpeed : walkSpeed);

        if (controller.isGrounded)
        {
            verticalVelocity = jump ? jumpSpeed : -stickToGroundForce;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        }

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }
}
