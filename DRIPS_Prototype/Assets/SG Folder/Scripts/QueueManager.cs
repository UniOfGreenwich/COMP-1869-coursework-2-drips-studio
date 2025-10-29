using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [SerializeField] private Transform queuePointsRoot;  
    [SerializeField] private Transform counterPoint;      
    [SerializeField] private int maxQueueSize = 5;

    private List<Transform> slots = new List<Transform>();
    private readonly List<CustomerController> queue = new List<CustomerController>();

    public int CurrentCount => queue.Count;
    public int MaxQueueSize => maxQueueSize;

    private void Awake()
    {
        slots.Clear();
        foreach (Transform child in queuePointsRoot)
            slots.Add(child);

        slots.Sort((a, b) => a.name.CompareTo(b.name));
    }

    public bool CanJoinQueue() => queue.Count < maxQueueSize;

    public bool TryJoinQueue(CustomerController customer)
    {
        if (!CanJoinQueue()) return false;
        queue.Add(customer);
        UpdateTargets();
        return true;
    }

    public void LeaveQueue(CustomerController customer)
    {
        if (queue.Remove(customer))
            UpdateTargets();
    }

    // Called when the front customer finishes "ordering"
    public void FrontCustomerDone()
    {
        if (queue.Count == 0) return;

        var front = queue[0];
        queue.RemoveAt(0);
        UpdateTargets();

        front.OnDequeuedFromFront(counterPoint.position);
    }

    private void UpdateTargets()
    {
        // Everyone re-targets their slot
        for (int i = 0; i < queue.Count; i++)
        {
            var customer = queue[i];
            if (i == 0)
                customer.SetQueueTarget(slots[0].position); 
            else if (i < slots.Count)
                customer.SetQueueTarget(slots[i].position);
            else
                customer.SetQueueTarget(slots[slots.Count - 1].position);
        }
    }
}
