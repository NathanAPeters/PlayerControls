
using System.Collections.Generic;
using UnityEngine;

static public class EggManager
{
    // ─── Global Stats ───────────────────────────────────────
    public static int EggScore = 0;              // Sum of all egg values collected
    public static int CollectedEggs = 0;          // Total number of eggs collected

    // ─── Per‑Scene Stats ────────────────────────────────────
    // Scene → how many eggs it contains (registered once)
    private static Dictionary<string, int> sceneEggTotals = new Dictionary<string, int>();
    // Scene → how many eggs have been collected in it
    private static Dictionary<string, int> sceneEggsCollected = new Dictionary<string, int>();

    // ─── Internal Tracking ──────────────────────────────────
    // Tracks which egg‑keys have been registered already
    private static HashSet<string> registeredEggKeys = new HashSet<string>();
    // Tracks which egg‑keys have been collected
    private static HashSet<string> collectedEggKeys = new HashSet<string>();

    // ─── Public API ─────────────────────────────────────────

    // Create a stable key for each egg: "SceneName_x_y_z"
    public static string GenerateEggKey(string sceneName, Vector3 position)
    {
        // Round pos to avoid tiny float differences
        Vector3 p = new Vector3(
            Mathf.Round(position.x * 100f) / 100f,
            Mathf.Round(position.y * 100f) / 100f,
            Mathf.Round(position.z * 100f) / 100f
        );
        return $"{sceneName}_{p.x}_{p.y}_{p.z}";
    }

    // Call once from each egg's Awake()
    public static void RegisterEgg(string sceneName, string eggKey)
    {
        // If this eggKey hasn't been registered yet, count it for the scene total
        if (registeredEggKeys.Add(eggKey))
        {
            if (!sceneEggTotals.ContainsKey(sceneName))
                sceneEggTotals[sceneName] = 0;
            sceneEggTotals[sceneName]++;
        }
    }

    // Call when the player actually collects the egg
    public static void CollectEgg(string sceneName, string eggKey, int value)
    {
        // Only collect once per eggKey
        if (collectedEggKeys.Add(eggKey))
        {
            EggScore += value;
            CollectedEggs++;

            if (!sceneEggsCollected.ContainsKey(sceneName))
                sceneEggsCollected[sceneName] = 0;
            sceneEggsCollected[sceneName]++;
        }
    }

    // Query helpers:
    public static bool HasEggBeenCollected(string eggKey)
        => collectedEggKeys.Contains(eggKey);

    public static int GetSceneEggsTotal(string sceneName)
        => sceneEggTotals.TryGetValue(sceneName, out var t) ? t : 0;

    public static int GetSceneEggsCollected(string sceneName)
        => sceneEggsCollected.TryGetValue(sceneName, out var c) ? c : 0;

    public static int GetTotalEggs()
    {
        int sum = 0;
        foreach (var kvp in sceneEggTotals)
            sum += kvp.Value;
        return sum;
    }

    // Debug display in Console
    public static void DebugLog(string sceneName)
    {
        int sceneTotal = GetSceneEggsTotal(sceneName);
        int sceneGot = GetSceneEggsCollected(sceneName);
        int globalTotal = GetTotalEggs();
        Debug.Log($"[Egg Debug] 🥚 Score: {EggScore} | Total Collected: {CollectedEggs}/{globalTotal} | " +
                  $"Scene '{sceneName}': {sceneGot}/{sceneTotal}");
    }
}