using System.Collections.Generic;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue")]
    [SerializeField] private Transform queuePointsRoot;  // child index 0 = ORDER spot, 1.. = waiting
    [SerializeField] private int maxQueueSize = 3;       // number waiting (excludes Slot_0)

    [Header("Order Spot Gating")]
    [Tooltip("Radius around Slot_0 that must be clear before promoting next.")]
    [SerializeField] private float orderSpotClearRadius = 0.6f;
    [Tooltip("Layers considered 'customers' for the clear check.")]
    [SerializeField] private LayerMask customerLayers = ~0;
    [Tooltip("Small delay after freeing Slot_0 before promoting the next.")]
    [SerializeField] private float promoteDelay = 0.5f;

    private readonly List<Transform> slots = new List<Transform>();
    private readonly List<CustomerController> queue = new List<CustomerController>();

    // 0 = free; 1 = someone moving/ordering at Slot_0 (logical lock)
    private int inFlightToCounter = 0;
    private float lastFreedTime = -999f;

    public int CurrentCount => queue.Count;
    public int MaxQueueSize => maxQueueSize;

    private void Awake()
    {
        inFlightToCounter = 0;
        queue.Clear();
        RefreshSlots();
    }

    private void OnValidate()
    {
        if (queuePointsRoot) RefreshSlots();
    }

    private void RefreshSlots()
    {
        slots.Clear();
        if (!queuePointsRoot)
        {
            Debug.LogError("[QueueManager] queuePointsRoot not assigned.");
            return;
        }

        for (int i = 0; i < queuePointsRoot.childCount; i++)
            slots.Add(queuePointsRoot.GetChild(i));

        if (slots.Count < maxQueueSize + 1)
            Debug.LogWarning($"[QueueManager] Need at least {maxQueueSize + 1} queue points (index 0 = ORDER, then {maxQueueSize} waiting).");
    }

    public bool CanJoinQueue() => queue.Count < maxQueueSize;

    public bool TryJoinQueue(CustomerController customer)
    {
        if (!CanJoinQueue()) return false;

        queue.Add(customer);
        UpdateTargets();
        TryPromoteToOrdering();
        return true;
    }

    public void LeaveQueue(CustomerController customer)
    {
        if (queue.Remove(customer))
        {
            UpdateTargets();
            TryPromoteToOrdering();
        }
    }

    // Called by the customer after finishing at Slot_0 (their 3s ordering)
    public void CounterFreed()
    {
        inFlightToCounter = Mathf.Max(0, inFlightToCounter - 1);
        lastFreedTime = Time.time;
        // don't promote immediately; let the previous customer start moving away
        Invoke(nameof(TryPromoteToOrdering), promoteDelay);
    }

    private bool IsOrderSpotClear()
    {
        if (slots.Count == 0) return true;
        Vector3 p = slots[0].position;
        // look for any colliders on "customerLayers" within the radius
        var hits = Physics.OverlapSphere(p, orderSpotClearRadius, customerLayers, QueryTriggerInteraction.Ignore);
        return hits == null || hits.Length == 0;
    }

    private void TryPromoteToOrdering()
    {
        // if we're already sending/serving someone at Slot_0, or nobody is waiting, stop.
        if (inFlightToCounter > 0) return;
        if (queue.Count == 0) return;

        // require the physical area at Slot_0 to be clear (and a tiny post-free delay)
        if (Time.time - lastFreedTime < promoteDelay) return;
        if (!IsOrderSpotClear()) return;

        // pop front of waiting queue and send them to Slot_0
        var front = queue[0];
        queue.RemoveAt(0);

        inFlightToCounter = 1;       // logical lock
        UpdateTargets();             // everyone else shifts up to indices 1,2,3...

        Vector3 orderingPos = slots[0].position; // child index 0 = ORDER spot
        front.OnGoToCounter(orderingPos);
    }

    private void UpdateTargets()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            int slotIndex = (inFlightToCounter > 0) ? (i + 1) : i; // reserve index 0 when someone is at/going to Slot_0
            slotIndex = Mathf.Min(slotIndex, slots.Count - 1);
            queue[i].SetQueueTarget(slots[slotIndex].position);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (queuePointsRoot && queuePointsRoot.childCount > 0)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(queuePointsRoot.GetChild(0).position, orderSpotClearRadius);
        }
    }
#endif
}
