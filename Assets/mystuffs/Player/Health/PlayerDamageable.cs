// PlayerDamageable.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class PlayerDamageable : MonoBehaviour
{
    // Maps each triggering Collider to its running damage coroutine
    private readonly Dictionary<Collider, Coroutine> activeCoroutines
        = new Dictionary<Collider, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        // Only proceed if the thing we collided with implements IDamageSource
        if (!other.TryGetComponent<IDamageSource>(out var src))
            return;

        // Avoid starting multiple coroutines for the same source
        if (activeCoroutines.ContainsKey(other))
            return;

        // 1) Reserve the entry so the coroutine sees its key immediately
        activeCoroutines[other] = null;

        // 2) Start the looping damage coroutine and store its handle
        activeCoroutines[other] = StartCoroutine(DamageLoop(other, src));
    }

    private void OnTriggerExit(Collider other)
    {
        // If we had a coroutine running for this collider, stop and remove it
        if (activeCoroutines.TryGetValue(other, out var routine))
        {
            if (routine != null)
                StopCoroutine(routine);

            activeCoroutines.Remove(other);
        }
    }

    private IEnumerator DamageLoop(Collider key, IDamageSource src)
    {
        int ticks = 0;

        // Keep applying damage until:
        // • the player leaves the trigger (we remove key in OnTriggerExit)
        // • or we reach the source’s MaxTicks (if >0)
        while (activeCoroutines.ContainsKey(key) &&
               (src.MaxTicks <= 0 || ticks < src.MaxTicks))
        {
            // Subtract health, clamped at zero
            PlayerStats.currentHealth =
                Mathf.Max(0f, PlayerStats.currentHealth - src.DamageAmount);

            ticks++;
            yield return new WaitForSeconds(src.TickInterval);
        }

        // If we ended because we hit maxTicks (not by exit), clean up
        activeCoroutines.Remove(key);
    }
}