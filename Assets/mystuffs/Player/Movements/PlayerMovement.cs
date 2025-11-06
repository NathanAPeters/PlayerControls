using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //charactercontroller for movement and crouch height
    private CharacterController controller;

    [Header("Walking & Running")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;

    [Header("Crouching")]
    public float playerHeight;
    public float crouchHeight = 1f;

    [Header("Crouch Transition")]
    public float crouchSpeed = 6f;
    private float targetHeight;
    private Vector3 targetCenter;

    [Header("Jumping Basic")]
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Ground Slam & Short Jump")]
    public float fallMultiplier = 1.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Visuals")]
    [Tooltip("Drag the capsule-mesh child here")]
    public Transform playerVisual;

    //internal keeping track of the direction to move
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (playerHeight == 0)
            playerHeight = controller.height;

        targetHeight = playerHeight;
        targetCenter = new Vector3(0, playerHeight / 2f, 0);
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    void Update()
    {
        // 1) Skip movement if climbing
        if (PlayerStats.isClimbing)
        {
            velocity = Vector3.zero;
            return;
        }

        // 2) Skip if movement locked
        if (!PlayerStats.canMove)
            return;

        // 3) Ground check
        bool isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 4) Horizontal movement
        float speed = Input.GetKey(PlayerStats.dashKeyBind) ? sprintSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // 5) Crouch
        bool isCrouching = Input.GetKey(PlayerStats.CrouchKeyBind);

        if (isCrouching)
        {
            targetHeight = crouchHeight;
            targetCenter = new Vector3(0, crouchHeight / 2f, 0);//prevents falling through floor
        }
        else
        {
            float checkDistance = playerHeight - crouchHeight;
            Vector3 startHeight = transform.position + Vector3.up * crouchHeight;

            //raycast to check above the player's head
            bool hasHeadClearance = !Physics.SphereCast(startHeight, controller.radius * 0.9f, Vector3.up, out _, checkDistance, ~0, QueryTriggerInteraction.Ignore);

            if (hasHeadClearance)
            {
                targetHeight = playerHeight;
                targetCenter = new Vector3(0, playerHeight / 2f, 0);
            }
            else
            {
                targetHeight = crouchHeight;
                targetCenter = new Vector3(0, crouchHeight / 2f, 0);
            }
        }

        // Smoothly interpolate height and center
        controller.height = Mathf.Lerp(controller.height, targetHeight, Time.deltaTime * crouchSpeed);
        controller.center = Vector3.Lerp(controller.center, targetCenter, Time.deltaTime * crouchSpeed);
        playerVisual.localPosition = controller.center;

        // 6) Jump
        if (PlayerStats.canJump && isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 7) Gravity + modifiers
        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else if (velocity.y > 0 && !Input.GetButton("Jump"))
        {
            velocity.y += gravity * lowJumpMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        controller.Move(velocity * Time.deltaTime);
    }
    public void Launch(Vector3 direction, float strength)
    {
        // To preserve horizontal momentum, comment out the next line:
        velocity = Vector3.zero;
        velocity += direction.normalized * strength;
    }
}