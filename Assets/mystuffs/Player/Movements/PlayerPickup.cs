using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    [SerializeField] private Transform holdArea;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float pickupRange = 5f;
    [SerializeField] private LayerMask pickupLayer;

    [Header("Spring Parameters")]
    [Tooltip("Higher → stronger pull toward holdArea")]
    [SerializeField] private float springForce = 500f;
    [Tooltip("Higher → more damping (less bounciness)")]
    [SerializeField] private float damper = 50f;
    [Tooltip("How far the spring can stretch before maxing out")]
    [SerializeField] private float maxDistance = 0.5f;

    [Header("Safety Net")]
    [Tooltip("If the held object drifts farther than this, teleport it home.")]
    [SerializeField] private float maxHoldDistance = 10f;

    private Rigidbody holdTargetRB;
    private GameObject heldObj;
    private Rigidbody heldRB;

    void Awake()
    {
        // Create a kinematic target at holdArea
        var go = new GameObject("HoldTarget");
        go.transform.SetParent(transform, false);
        holdTargetRB = go.AddComponent<Rigidbody>();
        holdTargetRB.isKinematic = true;
        holdTargetRB.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObj == null) TryPickup();
            else DropObject();
        }
    }

    void FixedUpdate()
    {
        // Move the target to follow your holdArea smoothly
        holdTargetRB.MovePosition(holdArea.position);
        holdTargetRB.MoveRotation(holdArea.rotation);

        // Safety net: teleport cube home if it drifts too far
        if (heldRB != null &&
            Vector3.Distance(heldRB.position, holdTargetRB.position) > maxHoldDistance)
        {
            heldRB.position = holdArea.position;
            heldRB.velocity = Vector3.zero;
            heldRB.angularVelocity = Vector3.zero;
        }
    }

    private void TryPickup()
    {
        if (Physics.Raycast(playerCamera.transform.position,
                            playerCamera.transform.forward,
                            out var hit, pickupRange, pickupLayer))
        {
            PickupObject(hit.collider.gameObject);
        }
    }

    private void PickupObject(GameObject obj)
    {
        var rb = obj.GetComponent<Rigidbody>();
        if (rb == null) return;

        heldObj = obj;
        heldRB = rb;

        // Basic physics tweaks
        heldRB.useGravity = false;
        heldRB.drag = 2f;
        heldRB.angularDrag = 10f;
        heldRB.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        heldRB.interpolation = RigidbodyInterpolation.Interpolate;

        // Add and configure a ConfigurableJoint
        var cj = heldObj.AddComponent<ConfigurableJoint>();
        cj.connectedBody = holdTargetRB;

        // Lock all motion so only our soft limit moves it
        cj.xMotion = ConfigurableJointMotion.Limited;
        cj.yMotion = ConfigurableJointMotion.Limited;
        cj.zMotion = ConfigurableJointMotion.Limited;
        cj.angularXMotion = cj.angularYMotion = cj.angularZMotion = ConfigurableJointMotion.Locked;

        // Set up the limit and spring
        cj.linearLimit = new SoftJointLimit { limit = maxDistance };
        cj.linearLimitSpring = new SoftJointLimitSpring
        {
            spring = springForce,
            damper = damper
        };

        // Projection helps keep it from jittering through corners
        cj.projectionMode = JointProjectionMode.PositionAndRotation;
        cj.projectionDistance = maxDistance * 0.5f;
        cj.projectionAngle = 1f;
    }

    private void DropObject()
    {
        if (heldObj != null)
        {
            // Destroy whichever joint we added
            var joint = heldObj.GetComponent<ConfigurableJoint>();
            if (joint != null) Destroy(joint);

            // Restore physics
            heldRB.useGravity = true;
            heldRB.drag = 0f;
            heldRB.angularDrag = 0.05f;
            heldRB.constraints = RigidbodyConstraints.None;

            heldObj = null;
            heldRB = null;
        }
    }
}