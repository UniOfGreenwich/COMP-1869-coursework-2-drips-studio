using System.Collections;
using UnityEngine;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private SeatingManager seatingManager;

    [Header("Spawn Control")]
    [Tooltip("Time between spawns (min/max in seconds).")]
    [SerializeField] private float spawnIntervalMin = 2f;
    [SerializeField] private float spawnIntervalMax = 5f;

    private int activeCustomers = 0;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // total customers already spawned or queued
            bool canSpawn = activeCustomers < queueManager.MaxQueueSize;

            if (canSpawn)
            {
                var go = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                var customer = go.GetComponent<CustomerController>();
                customer.Init(queueManager, seatingManager, exitPoint);
                activeCustomers++;

                // when destroyed, reduce count
                customer.StartCoroutine(DecreaseActiveCountWhenDestroyed(customer));
            }

            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(wait);
        }
    }

    private IEnumerator DecreaseActiveCountWhenDestroyed(CustomerController c)
    {
        while (c != null) yield return null;
        activeCustomers--;
    }
}
