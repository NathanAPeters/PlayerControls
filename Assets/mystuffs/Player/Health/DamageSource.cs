// DamageSource.cs
using UnityEngine;

public interface IDamageSource
{
    float DamageAmount { get; }
    float TickInterval { get; }
    int MaxTicks { get; }  // ≤0 = infinite until exit
}

[RequireComponent(typeof(Collider))]
public class DamageSource : MonoBehaviour, IDamageSource
{
    [Tooltip("How much health to subtract each tick")]
    public float damageAmount = 1f;

    [Tooltip("Seconds between damage ticks")]
    public float tickInterval = 0.5f;

    [Tooltip("Number of times to apply damage; ≤0 = infinite until you leave")]
    public int maxTicks = 0;

    // Interface properties
    public float DamageAmount => damageAmount;
    public float TickInterval => tickInterval;
    public int MaxTicks => maxTicks;

    private void Reset()
    {
        // Ensure collider is set to trigger so OnTriggerEnter fires
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }
}