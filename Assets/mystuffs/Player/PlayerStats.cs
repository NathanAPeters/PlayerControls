using UnityEngine;

public static class PlayerStats
{
    // Health
    public static float maxHealth = 10f;
    public static float currentHealth = 9f;
    public static float damage = 1f;

    // State flags
    public static bool isClimbing = false;
    public static bool canMove = true;
    public static bool canJump = true;

    //player movement
    public static KeyCode CrouchKeyBind = KeyCode.LeftControl;
    public static KeyCode JumpKeyBind = KeyCode.Space;

    //climbing
    public static KeyCode climbInteractKeyBind = KeyCode.E;
    public static KeyCode climbUpKeyBind = KeyCode.W;
    public static KeyCode climbDownKeyBind = KeyCode.S;

    // Key bindings
    public static KeyCode attackKeyBind = KeyCode.Mouse0;
    public static KeyCode dashKeyBind = KeyCode.LeftShift;
    public static KeyCode pickupItemKeyBind = KeyCode.E;

    // You can add more attack bindings as needed
    // public static KeyCode specialAttackKeyBind = KeyCode.Q;
}