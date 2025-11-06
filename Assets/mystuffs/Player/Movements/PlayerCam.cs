using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The transform of the player's body (usually the parent GameObject).")]
    [SerializeField] private Transform playerBody;
    [Tooltip("The CharacterController on the player (for reading current height).")]
    [SerializeField] private CharacterController controller;

    [Header("Mouse Settings")]
    [Tooltip("Look sensitivity multiplier.")]
    [SerializeField] private float mouseSensitivity = 2f;

    [Header("Camera Offsets")]
    [Tooltip("How much higher than the capsule center the camera sits (eye height).")]
    [SerializeField] private float cameraExtraOffset = 0.1f;

    // Internal state
    private float pitch = 0f;

    private void Start()
    {
        // Lock & hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Optional safety checks
        if (playerBody == null)
            Debug.LogError("PlayerCam: No playerBody assigned!");
        if (controller == null)
            Debug.LogError("PlayerCam: No CharacterController assigned!");
    }

    private void Update()
    {
        // --- LOOK INPUT ---

        // Vertical look (pitch)
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // Horizontal look (yaw)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void LateUpdate()
    {
        // --- APPLY ROTATION ---
        transform.rotation = Quaternion.Euler(pitch, playerBody.eulerAngles.y, 0f);

        // --- APPLY POSITION ---
        Vector3 playerPos = playerBody.position;
        float cameraY = playerPos.y + controller.height  + cameraExtraOffset;

        transform.position = new Vector3(playerPos.x, cameraY, playerPos.z);
    }
}