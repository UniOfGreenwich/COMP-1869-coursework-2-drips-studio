using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue")]
    [SerializeField] private Transform queuePointsRoot;  // parent with Slot_0..Slot_N
    [SerializeField] private int maxQueueSize = 3;

    [Header("Counter")]
    [SerializeField] private Transform counterPoint;     // reachable point on NavMesh in front of counter
    private bool counterBusy = false;

    private readonly List<Transform> slots = new List<Transform>();
    private readonly List<CustomerController> queue = new List<CustomerController>();

    public int CurrentCount => queue.Count;
    public int MaxQueueSize => maxQueueSize;

    private static int NaturalIndex(Transform t)
    {
        // Parse trailing number from name like "Slot_0", "Slot_12"
        var m = Regex.Match(t.name, @"(\d+)$");
        return m.Success ? int.Parse(m.Value) : int.MaxValue;
    }

    private void Awake() => RefreshSlots();

    private void OnValidate()
    {
        // Keep editor-time updates accurate
        if (queuePointsRoot != null)
            RefreshSlots();
    }

    private void RefreshSlots()
    {
        slots.Clear();
        if (queuePointsRoot == null) return;

        foreach (Transform child in queuePointsRoot)
            slots.Add(child);

        // Natural numeric sort so Slot_10 doesn't come before Slot_2
        slots.Sort((a, b) => NaturalIndex(a).CompareTo(NaturalIndex(b)));

        if (slots.Count < maxQueueSize)
        {
            Debug.LogWarning($"[QueueManager] You set MaxQueueSize={maxQueueSize} but only defined {slots.Count} slot(s) under '{queuePointsRoot.name}'. " +
                             $"Customers beyond {slots.Count} will reuse the last slot and overlap. Create Slot_0..Slot_{maxQueueSize - 1}.");
        }
    }

    public bool CanJoinQueue() => queue.Count < maxQueueSize;

    public bool TryJoinQueue(CustomerController customer)
    {
        if (!CanJoinQueue()) return false;

        queue.Add(customer);
        UpdateTargets();
        TryPromoteFrontToCounter(); // <-- key: auto-promote when possible
        return true;
    }

    public void LeaveQueue(CustomerController customer)
    {
        if (queue.Remove(customer))
        {
            UpdateTargets();
            TryPromoteFrontToCounter();
        }
    }

    // Called by the current counter customer when they finish ordering
    public void CounterFreed()
    {
        counterBusy = false;
        TryPromoteFrontToCounter();
    }

    private void TryPromoteFrontToCounter()
    {
        if (counterBusy) return;
        if (queue.Count == 0) return;

        counterBusy = true;
        var front = queue[0];
        queue.RemoveAt(0);
        UpdateTargets();

        // send to counter approach point
        front.OnGoToCounter(counterPoint.position);
    }

    private void UpdateTargets()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            int slotIndex = Mathf.Min(i, slots.Count - 1);
            var target = (slots.Count > 0) ? slots[slotIndex].position : counterPoint.position;
            queue[i].SetQueueTarget(target);
        }
    }

    // --- Gizmos to visualise your queue in the Scene view ---
    private void OnDrawGizmos()
    {
        if (queuePointsRoot == null) return;
        Gizmos.color = Color.cyan;

        int i = 0;
        foreach (Transform child in queuePointsRoot)
        {
            Gizmos.DrawWireSphere(child.position, 0.15f);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(child.position + Vector3.up * 0.1f, child.name);
#endif
            i++;
        }

        if (counterPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(counterPoint.position, Vector3.one * 0.2f);
#if UNITY_EDITOR
            UnityEditor.Handles.Label(counterPoint.position + Vector3.up * 0.1f, "Counter");
#endif
        }
    }
}
