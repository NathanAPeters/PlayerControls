using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float jumpStrength = 10f;
    [SerializeField] private Vector3 launchDirection = Vector3.up;

    private void OnTriggerEnter(Collider other)
    {
        // look for our PlayerMovement component (not SimplePlayerMovement)
        if (other.TryGetComponent<SimplePlayerMovement>(out var movement))
        //if (other.TryGetComponent<PlayerMovement>(out var movement))
        {
            movement.Launch(launchDirection, jumpStrength);
        }
    }
}