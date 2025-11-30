using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IC_SplatterManager : MonoBehaviour
{
    [Header("Prefab to Spawn")]
    [SerializeField] private GameObject splatterPrefab;

    [Header("Spawn Timing (seconds)")]
    [SerializeField] private float minSpawnDelay = 30f;
    [SerializeField] private float maxSpawnDelay = 120f;

    [Header("Spawn Settings")]
    [SerializeField] private bool randomRotation = true;
    [SerializeField] private bool destroyPlaceholders = true;

    private List<Vector3> splatterPositions = new List<Vector3>();

    private void Start()
    {
        foreach (Transform child in transform)
        {
            splatterPositions.Add(child.position);
        }

        if (destroyPlaceholders)
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        // Start the spawn loop
        StartCoroutine(SplatterLoop());
    }

    private IEnumerator SplatterLoop()
    {
        while (GameManager.Instance.open)
        {
            // Wait a random amount of time between spawns
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            SpawnRandomSplatter();
        }
    }

    private void SpawnRandomSplatter()
    {
        if (splatterPrefab == null || splatterPositions.Count == 0)
        {
            Debug.LogWarning("SplatterManager: Missing prefab or no positions found.");
            return;
        }

        // Choose a random spawn position
        Vector3 spawnPos = splatterPositions[Random.Range(0, splatterPositions.Count)];

        // Random rotation (Z-axis)
        Quaternion rotation = randomRotation
            ? Quaternion.Euler(0, 0, Random.Range(0f, 360f))
            : Quaternion.identity;

        Instantiate(splatterPrefab, spawnPos, rotation, transform);
    }
}
