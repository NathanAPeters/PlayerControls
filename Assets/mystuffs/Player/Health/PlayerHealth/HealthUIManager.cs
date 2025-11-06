using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    [Header("Prefabs (UI Images)")]
    public GameObject fullHeartUI;
    public GameObject halfHeartUI;
    public GameObject emptyHeartUI;

    private Transform healthContainer;
    private List<GameObject> heartUIs = new List<GameObject>();
    private float lastHealth = -1f;
    private float lastMaxHealth = -1f;

    void Start()
    {
        // Find the container by tag at startup
        var containerGO = GameObject.FindWithTag("HealthContainer");
        if (containerGO == null)
        {
            Debug.LogError("No GameObject found with tag 'HealthContainer'");
            return;
        }
        healthContainer = containerGO.transform;

        UpdateUI();
    }

    void Update()
    {
        if (PlayerStats.currentHealth != lastHealth || PlayerStats.maxHealth != lastMaxHealth)
            UpdateUI();
    }

    void UpdateUI()
    {
        lastHealth = PlayerStats.currentHealth;
        lastMaxHealth = PlayerStats.maxHealth;

        // Clear existing hearts
        foreach (var go in heartUIs)
            Destroy(go);
        heartUIs.Clear();

        int maxH = Mathf.RoundToInt(lastMaxHealth);
        float currH = lastHealth;

        for (int i = 0; i < maxH; i++)
        {
            GameObject prefabToUse;

            if (i < currH - 0.5f)
                prefabToUse = fullHeartUI;
            else if (i < currH)
                prefabToUse = halfHeartUI;
            else
                prefabToUse = emptyHeartUI;

            var heartGO = Instantiate(prefabToUse, healthContainer);
            heartUIs.Add(heartGO);
        }
    }
}