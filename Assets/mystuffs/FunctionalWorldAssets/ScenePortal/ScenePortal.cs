using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    [SerializeField] string sceneToLoad;

    private void Start()
    {
        // Delay a tick so all Awake() registrations finish
        Invoke(nameof(LogEggStats), 0.1f);
    }

    private void LogEggStats()
    {
        string scene = SceneManager.GetActiveScene().name;
        EggManager.DebugLog(scene);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            SceneManager.LoadScene(sceneToLoad);
    }
}