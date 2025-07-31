using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintMultiplier = 1.7f;
    [SerializeField] float jumpHeight = 1.2f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float lookSensitivity = 1.0f;

    CharacterController cc;
    PlayerInput input;

    Vector2 move;
    Vector2 look;
    Vector3 velocity;
    float pitch;                      // camera up/down

    [SerializeField] Transform cameraPivot;  // drag MainCamera or the cam root here

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = GetComponent<PlayerInput>();
    }

    void OnEnable()
    {
        // === Movement ===
        input.actions["Player/Move"].performed += ctx => move = ctx.ReadValue<Vector2>();
        input.actions["Player/Move"].canceled += _ => move = Vector2.zero;

        // === Look ===
        input.actions["Player/Look"].performed += ctx => look = ctx.ReadValue<Vector2>();
        input.actions["Player/Look"].canceled += _ => look = Vector2.zero;

        // === Jump ===
        input.actions["Player/Jump"].performed += _ => Jump();
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
        ApplyGravity();
    }

    // ---------- Movement ----------
    void HandleMovement()
    {
        bool sprintHeld = input.actions["Player/Sprint"].IsPressed();
        float speed = sprintHeld ? walkSpeed * sprintMultiplier : walkSpeed;

        Vector3 dir = (transform.right * move.x + transform.forward * move.y).normalized;
        cc.Move(dir * speed * Time.deltaTime);
    }

    // ---------- Mouse-look ----------
    void HandleLook()
    {
        float yaw = look.x * lookSensitivity;
        float pitchDelta = -look.y * lookSensitivity;   // invert if you like
        transform.Rotate(Vector3.up * yaw, Space.World);

        pitch = Mathf.Clamp(pitch + pitchDelta, -80f, 80f);
        cameraPivot.localEulerAngles = new Vector3(pitch, 0, 0);
    }

    // ---------- Jump / Gravity ----------
    void Jump()
    {
        if (IsGrounded())
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    bool IsGrounded()
    {
        // CharacterController.isGrounded can miss fast slopes, so add a tiny raycast
        return cc.isGrounded || Physics.Raycast(transform.position, Vector3.down,
                                                cc.height / 2f + 0.05f);
    }

    void ApplyGravity()
    {
        if (IsGrounded() && velocity.y < 0) velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
