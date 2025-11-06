using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonDoor : MonoBehaviour
{
    [SerializeField] private GameObject door;
    [SerializeField] private Vector3 moveOffset = new Vector3(0, 2, 0);
    [SerializeField] private float moveSpeed = 2f; // units per second

    private GameObject player;
    private int numberOfItems;
    private Vector3 closedPosition;
    private Vector3 openPosition;

    private Coroutine movementCoroutine;
    private Vector3 targetPosition;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        closedPosition = door.transform.position;
        openPosition = closedPosition + moveOffset;
        targetPosition = closedPosition;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player || other.CompareTag("Grabbable"))
        {
            numberOfItems++;
            if (numberOfItems == 1) // only trigger when the first item enters
                SetTarget(openPosition);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player || other.CompareTag("Grabbable"))
        {
            numberOfItems = Mathf.Max(0, numberOfItems - 1);
            if (numberOfItems == 0)
                SetTarget(closedPosition);
        }
    }

    private void SetTarget(Vector3 newTarget)
    {
        if (targetPosition != newTarget)
        {
            targetPosition = newTarget;
            if (movementCoroutine != null)
                StopCoroutine(movementCoroutine);
            movementCoroutine = StartCoroutine(MoveDoor());
        }
    }

    private IEnumerator MoveDoor()
    {
        while ((door.transform.position - targetPosition).sqrMagnitude > 0.001f)
        {
            door.transform.position = Vector3.MoveTowards(
                door.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        door.transform.position = targetPosition;
        movementCoroutine = null;
    }
}