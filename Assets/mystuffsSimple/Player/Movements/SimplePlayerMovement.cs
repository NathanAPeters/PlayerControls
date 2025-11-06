using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 velocity;

    public float speed = 5f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;
    public float defaultyYVelocity = -2f;

    public float standingHeight = 2f;
    public float crouchingHeight = 1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        controller.height = standingHeight;
    }

    void Update()
    {
        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = defaultyYVelocity;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Jump
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
            velocity.y = Mathf.Sqrt(jumpHeight * defaultyYVelocity * gravity);

        // Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
            controller.height = crouchingHeight;
        else if (Input.GetKeyUp(KeyCode.LeftControl))
            controller.height = standingHeight;
    }

    //later
    public void Launch(Vector3 direction, float strength)
    {
        velocity.y = 0f; // optional reset
        velocity += direction.normalized * strength;
    }
}
