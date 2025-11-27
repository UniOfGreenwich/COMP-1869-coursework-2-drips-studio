using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue Points (child index 0 = counter spot)")]
    [SerializeField] private Transform queuePointsRoot;
    [SerializeField] private int maxQueueSize = 3;   // number waiting (not counting the person at counter)
    public int MaxQueueSize => maxQueueSize;


    private readonly List<Transform> slots = new List<Transform>(); // [0]=counter, [1..]=queue
    private readonly List<CustomerController> waiting = new List<CustomerController>();
    private CustomerController orderingCustomer = null;

    public bool CanJoinQueue() => waiting.Count < maxQueueSize;

    private void Awake() => RefreshSlots();
    private void OnValidate() { if (queuePointsRoot) RefreshSlots(); }

    private void RefreshSlots()
    {
        slots.Clear();
        if (!queuePointsRoot) return;
        for (int i = 0; i < queuePointsRoot.childCount; i++)
            slots.Add(queuePointsRoot.GetChild(i));
    }

    // Called by spawner/controller right after spawn
    public bool TryJoinQueue(CustomerController c)
    {
        // If counter free → send straight to counter (FIRST CUSTOMER GOES NOW)
        if (orderingCustomer == null)
        {
            orderingCustomer = c;
            c.OnGoToCounter(slots[0].position);
            UpdateTargets(); // push any existing waiters down (there probably aren't any yet)
            return true;
        }

        // Otherwise, join waiting line (if there’s room)
        if (!CanJoinQueue()) return false;

        waiting.Add(c);
        // tell them where to stand (Slot_1, Slot_2, ...)
        UpdateTargets();
        return true;
    }

    // Called by the customer after they finish ordering
    public void CounterFreed()
    {
        orderingCustomer = null;
        PromoteIfPossible();
    }

    private void PromoteIfPossible()
    {
        if (orderingCustomer != null) return;
        if (waiting.Count == 0) return;

        // pop front → send to counter
        orderingCustomer = waiting[0];
        waiting.RemoveAt(0);
        UpdateTargets();
        orderingCustomer.OnGoToCounter(slots[0].position);
    }

    private void UpdateTargets()
    {
        // If someone is at counter, first waiter goes to Slot_1; otherwise Slot_0
        for (int i = 0; i < waiting.Count; i++)
        {
            int slotIndex = (orderingCustomer != null) ? (i + 1) : i;
            slotIndex = Mathf.Min(slotIndex, slots.Count - 1);
            waiting[i].SetQueueTarget(slots[slotIndex].position);
        }
    }
}
