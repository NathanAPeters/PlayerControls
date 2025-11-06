using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ClimbLadder : MonoBehaviour
{
    [Header("How fast you climb (units/sec)")]
    [SerializeField] private float climbSpeed = 3f;

    [Header("Assign two child Transforms to mark the top/bottom Y positions")]
    [SerializeField] private Transform topPoint;
    [SerializeField] private Transform bottomPoint;
    // A tiny offest allowing the player to snap above the bottom and below the top
    [SerializeField] private float pointOffset = 0.3f;

    [Header("UI for 'Press E to Climb'")]
    [SerializeField] private GameObject climbUI;

    [Header("UI for 'Press E to Get Off' (shown only while on ladder)")]
    [SerializeField] private GameObject exitUI;

    

    private CharacterController playerController;
    private PlayerMovement playerMoveScript;
    private bool playerInTrigger = false;
    private bool isClimbing = false;

    private void Start()
    {
        // Hide both UI prompts at startup
        if (climbUI != null)
            climbUI.SetActive(false);
        else
            Debug.LogWarning("[ClimbLadder] climbUI not assigned in Inspector.");

        if (exitUI != null)
            exitUI.SetActive(false);
        else
            Debug.LogWarning("[ClimbLadder] exitUI not assigned in Inspector.");
    }

    private void Update()
    {
        // 1) If player is in the ladder trigger AND not already climbing, pressing E → attach
        if (playerInTrigger && !isClimbing && Input.GetKeyDown(PlayerStats.climbInteractKeyBind))
        {
            EnterLadder();
        }
        else if(playerInTrigger && isClimbing && Input.GetKeyDown(PlayerStats.climbInteractKeyBind))
        {
            ExitLadder();
        }

        // 2) If player is currently climbing, handle up/down movement
        if (isClimbing && playerController != null)
        {
            float verticalInput = Input.GetAxisRaw("Vertical"); // W/S or Up/Down arrows

            // Disable normal movement & jumping, implicitly turning off gravity
            PlayerStats.canMove = false;
            PlayerStats.canJump = false;
            PlayerStats.isClimbing = true;

            // Move strictly in Y (we freeze horizontal in CharacterController Move call)
            Vector3 climbDelta = Vector3.up * (verticalInput * climbSpeed * Time.deltaTime);
            playerController.Move(climbDelta);

            // If no vertical input, zero any leftover Y velocity (prevents sliding)
            if (Mathf.Approximately(verticalInput, 0f))
                playerMoveScript.ResetVelocity();

            // If the player's Y is now beyond the top or bottom, automatically exit
            float playerY = playerController.transform.position.y;
            if (playerY > topPoint.position.y + pointOffset ||
                playerY < bottomPoint.position.y - pointOffset)
            {
                ExitLadder();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only react if the collider belongs to “Player” (by tag)
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;

            // Setting the climb UI
            if (!isClimbing && climbUI != null)
            {
                climbUI.SetActive(true);
            }

            // Cache references from the Player object
            playerController = other.GetComponent<CharacterController>();
            playerMoveScript = other.GetComponent<PlayerMovement>();

            if (playerController == null)
                Debug.LogError("[ClimbLadder] CharacterController missing on Player!");
            if (playerMoveScript == null)
                Debug.LogError("[ClimbLadder] PlayerMovement missing on Player!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;

            //  Only hide climbUI if we are NOT climbing 
            if (!isClimbing && climbUI != null)
            {
                climbUI.SetActive(false);
            }

            // If they walk off while still climbing, force an exit
            if (isClimbing)
                ExitLadder();

            // Clear references so we don’t accidentally hold onto them
            playerController = null;
            playerMoveScript = null;
        }
    }

    private void EnterLadder()
    {
        if (playerController == null || playerMoveScript == null)
        {
            Debug.LogError("[ClimbLadder] Cannot EnterLadder: Missing references!");
            return;
        }

        // 1) Figure out whether to snap to just above bottom or snap exactly to top:
        float midY = (topPoint.position.y + bottomPoint.position.y) * 0.5f;
        float playerY = playerController.transform.position.y;
        Vector3 snapPos = playerController.transform.position;

        if (playerY < midY)
        {
            // Snap to EXACTLY bottomPoint.x/z, and Y just above bottomPoint.y
            snapPos.x = bottomPoint.position.x;
            snapPos.z = bottomPoint.position.z;
            snapPos.y = bottomPoint.position.y + pointOffset;
        }
        else
        {
            // Snap to EXACTLY topPoint.x/z, and Y exactly topPoint.y
            snapPos.x = topPoint.position.x;
            snapPos.z = topPoint.position.z;
            snapPos.y = topPoint.position.y;
        }

        // TEMPORARILY disable the CharacterController so the manual teleport “sticks”
        playerController.enabled = false;
        playerController.transform.position = snapPos;
        playerController.enabled = true;

        // 2) Enter climbing state
        isClimbing = true;
        PlayerStats.isClimbing = true;
        PlayerStats.canMove = false;
        PlayerStats.canJump = false;

        // Zero out any current motion
        playerMoveScript.ResetVelocity();

        // Hide the “Press E to Climb” UI now that we’re attached
        if (climbUI != null)
            climbUI.SetActive(false);

        // Show the “Press E to Get Off” UI
        if (exitUI != null)
            exitUI.SetActive(true);
    }

    private void ExitLadder()
    {
        // 1) Exit climbing state
        isClimbing = false;
        PlayerStats.isClimbing = false;
        PlayerStats.canMove = true;
        PlayerStats.canJump = true;

        // 2) Hide the “Press E to Get Off” UI
        if (exitUI != null)
            exitUI.SetActive(false);

        // 3) Zero any leftover Y‐velocity so we don’t “fly” off
        if (playerMoveScript != null)
            playerMoveScript.ResetVelocity();

        // 4) If still inside the ladder trigger, re‐show the “Press E to Climb” UI
        if (playerInTrigger && climbUI != null)
            climbUI.SetActive(true);
    }
}