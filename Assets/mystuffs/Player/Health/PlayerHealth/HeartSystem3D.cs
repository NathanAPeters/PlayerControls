using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSystem3D : MonoBehaviour
{
    [Header("Heart Prefabs")]
    public GameObject fullHeartPrefab;
    public GameObject halfHeartPrefab;
    public GameObject emptyHeartPrefab;

    [Header("Layout")]
    public float spacing = 0.5f;

    private List<GameObject> heartObjects = new List<GameObject>();
    private float lastHealth = -1f;
    private float lastMaxHealth = -1f;

    void Start()
    {
        // Initial draw
        UpdateHearts();
        lastHealth = PlayerStats.currentHealth;
        lastMaxHealth = PlayerStats.maxHealth;
    }

    void Update()
    {
        // Only rebuild when health or maxHealth has changed
        if (PlayerStats.currentHealth != lastHealth || PlayerStats.maxHealth != lastMaxHealth)
        {
            UpdateHearts();
            lastHealth = PlayerStats.currentHealth;
            lastMaxHealth = PlayerStats.maxHealth;
        }
    }

    void UpdateHearts()
    {
        // Destroy old hearts
        foreach (var heart in heartObjects)
            Destroy(heart);
        heartObjects.Clear();

        int maxH = Mathf.RoundToInt(PlayerStats.maxHealth);
        float currH = PlayerStats.currentHealth;

        for (int i = 0; i < maxH; i++)
        {
            GameObject prefabToUse;

            if (i < currH - 0.5f)
                prefabToUse = fullHeartPrefab;
            else if (i < currH)
                prefabToUse = halfHeartPrefab;
            else
                prefabToUse = emptyHeartPrefab;

            var heart = Instantiate(prefabToUse, transform);
            heart.transform.localPosition = new Vector3(i * spacing, 0, 0);
            heartObjects.Add(heart);
        }
    }
}