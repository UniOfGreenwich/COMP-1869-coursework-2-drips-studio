using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    public enum State
    {
        Spawning,
        Queueing,
        MovingToCounter,
        Ordering,
        FindingSeat,
        MovingToSeat,
        Sitting,
        Leaving
    }

    [Header("Refs")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private SeatingManager seatingManager;

    [Header("Timings")]
    [SerializeField] private float orderDuration = 3f;
    [SerializeField] private float sitDuration = 30f;

    [Header("Arrival")]
    [SerializeField] private float arriveDistance = 0.6f;

    private NavMeshAgent agent;
    private State state;
    private Vector3 queueTarget;
    private Seat mySeat;

    private void Awake() => agent = GetComponent<NavMeshAgent>();

    public void Init(QueueManager qm, SeatingManager sm, Transform exit)
    {
        queueManager = qm;
        seatingManager = sm;
        exitPoint = exit;
        StartCoroutine(Main());
    }

    // QueueManager will call these:
    public void SetQueueTarget(Vector3 pos)
    {
        queueTarget = pos;
        if (state != State.Queueing)
        {
            state = State.Queueing;
        }
        MoveTo(queueTarget);
    }

    public void OnGoToCounter(Vector3 counterPos)
    {
        state = State.MovingToCounter;
        MoveTo(counterPos);
    }

    private IEnumerator Main()
    {
        // Try join queue. If queue full, just leave.
        if (!queueManager.TryJoinQueue(this))
        {
            state = State.Leaving;
        }

        // Wait in queue until promoted
        while (state == State.Queueing) yield return null;

        // Move to counter and wait until we’ve arrived
        while (state == State.MovingToCounter && !Arrived())
            yield return null;

        // Order (3s)
        state = State.Ordering;
        yield return new WaitForSeconds(orderDuration);
        queueManager.CounterFreed();

        // Find a seat
        state = State.FindingSeat;
        if (seatingManager.TryGetFreeSeat(out mySeat) && mySeat != null)
        {
            state = State.MovingToSeat;
            MoveTo(mySeat.transform.position);
            while (!Arrived()) yield return null;

            state = State.Sitting;
            yield return new WaitForSeconds(sitDuration);

            seatingManager.ReleaseSeat(mySeat);
            mySeat = null;
        }

        // Leave
        state = State.Leaving;
        MoveTo(exitPoint.position);
        // just give them time to walk off; then despawn
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    private void MoveTo(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh) agent.SetDestination(pos);
    }

    private bool Arrived()
    {
        if (agent == null || agent.pathPending) return false;
        if (agent.remainingDistance <= Mathf.Max(arriveDistance, agent.stoppingDistance)) return true;
        return false;
    }
}

