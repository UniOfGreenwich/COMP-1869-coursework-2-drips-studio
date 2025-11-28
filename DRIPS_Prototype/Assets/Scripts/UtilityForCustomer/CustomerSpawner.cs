using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [SerializeField] private int activeCustomers = 0;

    public IEnumerator InitLoop()
    {
        bool init = true;
        while (init)
        {
            GameObject go;
            CustomerController customer = null;
            // total customers already spawned or queued
            bool canSpawn = activeCustomers < queueManager.MaxQueueSize;

            if (canSpawn)
            {
                go = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                customer = go.GetComponent<CustomerController>();
                customer.Init(queueManager, seatingManager, exitPoint);
                activeCustomers++;
                float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
                yield return new WaitForSeconds(wait);
            }
            if(!canSpawn)
            {
                init = false;
            }
            yield return StartCoroutine(SpawnLoop());
        }
        yield return null;
    }

    public IEnumerator SpawnLoop()
    {
        while (GameManager.Instance.open)
        {
            GameObject go;
            CustomerController customer;
            GameObject[] customerInScene = GameObject.FindGameObjectsWithTag("Customer");
            // total customers already spawned or queued
            bool canSpawn = activeCustomers < queueManager.MaxQueueSize;

            if (canSpawn)
            {
                if (customerInScene.Length < queueManager.MaxQueueSize)
                {
                    go = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                    customer = go.GetComponent<CustomerController>();
                    customer.Init(queueManager, seatingManager, exitPoint);
                    activeCustomers++;
                }                
            }
            while (!canSpawn)
            {
                // when destroyed, reduce count
                if (activeCustomers >= queueManager.MaxQueueSize / 2)
                {
                    if (customerInScene.Length == queueManager.MaxQueueSize)
                    {
                        activeCustomers--;
                        canSpawn = true;
                    }
                }
                else
                {
                    canSpawn = true;
                }
            }

            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            yield return new WaitForSeconds(wait);
        }
        GameObject[] customerInSceneEnd = GameObject.FindGameObjectsWithTag("Customer");
        foreach (GameObject customer in customerInSceneEnd)
        {
            Destroy(customer);
        }
        yield return null;
    }
}
