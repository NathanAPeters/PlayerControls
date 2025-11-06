using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayerPickup : MonoBehaviour
{
    public Transform holdPoint;
    public float pickupRange = 3f;
    public Camera cam;

    private GameObject heldObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                TryPickup();
            else
                Drop();
        }
    }

    void TryPickup()
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                heldObject = hit.collider.gameObject;
                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = Vector3.zero;
                if (heldObject.TryGetComponent<Rigidbody>(out var rb))
                    rb.isKinematic = true;
            }
        }
    }

    void Drop()
    {
        if (heldObject != null)
        {
            heldObject.transform.SetParent(null);
            if (heldObject.TryGetComponent<Rigidbody>(out var rb))
                rb.isKinematic = false;
            heldObject = null;
        }
    }
}