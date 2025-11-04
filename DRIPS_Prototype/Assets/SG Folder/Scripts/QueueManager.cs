using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class QueueManager : MonoBehaviour
{
    [Header("Queue")]
    [SerializeField] private Transform queuePointsRoot;  // Slot_0 = ORDER, Slot_1.. = waiting
    [SerializeField] private int maxQueueSize = 3;       // number waiting (excludes Slot_0)

    private readonly List<Transform> slots = new List<Transform>();
    private readonly List<CustomerController> queue = new List<CustomerController>();

    // 0 = free; 1 = someone moving/ordering at Slot_0
    private int inFlightToCounter = 0;
    private float lockSetTime = 0f;

    public int CurrentCount => queue.Count;
    public int MaxQueueSize => maxQueueSize;

    private static int NaturalIndex(Transform t)
    {
        var m = Regex.Match(t.name, @"(\d+)$");
        return m.Success ? int.Parse(m.Value) : int.MaxValue;
    }

    private void Awake()
    {
        inFlightToCounter = 0;       // HARD RESET every play
        lockSetTime = 0f;
        queue.Clear();
        RefreshSlots();
        Debug.Log($"[QueueManager:{GetInstanceID()}] Awake – inFlight=0, Slots={slots.Count}, GO={name}");
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
        foreach (Transform child in queuePointsRoot) slots.Add(child);
        slots.Sort((a, b) => NaturalIndex(a).CompareTo(NaturalIndex(b)));

        if (slots.Count < maxQueueSize + 1)
            Debug.LogWarning($"[QueueManager] Need at least {maxQueueSize + 1} Slot_* (Slot_0 + {maxQueueSize} waiting).");
    }

    public bool CanJoinQueue() => queue.Count < maxQueueSize;

    public bool TryJoinQueue(CustomerController customer)
    {
        if (!CanJoinQueue()) return false;

        queue.Add(customer);

        // --- SELF-HEAL: if the VERY FIRST join finds the lock stuck, unlock it ---
        if (queue.Count == 1 && inFlightToCounter > 0)
        {
            Debug.LogWarning($"[QueueManager:{GetInstanceID()}] Detected stuck lock on first join. Auto-unlocking.");
            inFlightToCounter = 0;
        }

        Debug.Log($"[QueueManager:{GetInstanceID()}] Joined queue. Waiting={queue.Count}, inFlight={inFlightToCounter}");
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

    // Called by the customer after finishing at Slot_0
    public void CounterFreed()
    {
        inFlightToCounter = Mathf.Max(0, inFlightToCounter - 1);
        Debug.Log($"[QueueManager:{GetInstanceID()}] Slot_0 freed. inFlight={inFlightToCounter}");
        TryPromoteToOrdering();
    }

    private void TryPromoteToOrdering()
    {
        // SECOND SELF-HEAL: if lock has been held too long without promotions, release it.
        if (inFlightToCounter > 0 && Time.time - lockSetTime > 5f)
        {
            Debug.LogWarning($"[QueueManager:{GetInstanceID()}] Lock held too long. Forcing unlock.");
            inFlightToCounter = 0;
        }

        if (inFlightToCounter > 0)
        {
            Debug.Log($"[QueueManager:{GetInstanceID()}] Cannot promote: inFlight={inFlightToCounter}");
            return;
        }
        if (queue.Count == 0)
        {
            // nothing to do
            return;
        }

        // pop front of waiting queue and send to Slot_0
        var front = queue[0];
        queue.RemoveAt(0);

        inFlightToCounter = 1;
        lockSetTime = Time.time;

        UpdateTargets(); // others shift up to Slot_1, Slot_2, ...

        var orderingPos = slots[0].position; // Slot_0
        Debug.Log($"[QueueManager:{GetInstanceID()}] Promoting '{front.name}' to Slot_0");
        front.OnGoToCounter(orderingPos);
    }

    private void UpdateTargets()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            int slotIndex = (inFlightToCounter > 0) ? (i + 1) : i;  // reserve Slot_0 if locked
            slotIndex = Mathf.Min(slotIndex, slots.Count - 1);
            queue[i].SetQueueTarget(slots[slotIndex].position);
        }
    }
}
