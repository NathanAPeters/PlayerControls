using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectibleEgg : MonoBehaviour
{
    [SerializeField] int value = 1;

    private string eggKey;
    private string sceneName;

    private void Awake()
    {
        sceneName = SceneManager.GetActiveScene().name;
        eggKey = EggManager.GenerateEggKey(sceneName, transform.position);

        // Register this egg for the scene's total (only ever increments once)
        EggManager.RegisterEgg(sceneName, eggKey);

        // If it was already collected earlier, hide it now
        if (EggManager.HasEggBeenCollected(eggKey))
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !EggManager.HasEggBeenCollected(eggKey))
        {
            // Collect it!
            EggManager.CollectEgg(sceneName, eggKey, value);
            EggManager.DebugLog(sceneName);

            // And disappear
            gameObject.SetActive(false);
        }
    }
}