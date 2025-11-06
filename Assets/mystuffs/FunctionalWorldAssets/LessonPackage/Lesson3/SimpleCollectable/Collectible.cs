using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] int value;
    [SerializeField] CollectibleManager man;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            man.amount += value;
            man.showAmount();
            Destroy(gameObject);
        }
    }
}
