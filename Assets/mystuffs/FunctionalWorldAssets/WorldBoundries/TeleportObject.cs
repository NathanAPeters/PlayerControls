using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToTeleport;
    [SerializeField] Transform teleportPosition;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var item in objectsToTeleport)
        {
            if (other.gameObject == item)
            {
                // Try to get the character controller
                CharacterController cc = item.GetComponent<CharacterController>();
                if (cc != null)//there is one
                    cc.enabled = false;

                //once you add health
                SimplePlayerHealth health = item.GetComponent<SimplePlayerHealth>();
                if (health != null)
                    health.TakeDamage(25);
                // Teleport
                item.transform.position = teleportPosition.position;

                // Re-enable the controller
                if (cc != null)
                    cc.enabled = true;
            }
        }
        
    }
}