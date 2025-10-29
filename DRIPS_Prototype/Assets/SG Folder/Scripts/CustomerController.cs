using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    public enum State
    {
        None, Spawning, Queueing, MovingToCounter, Ordering, FindingSeat, MovingToSeat, Sitting, Leaving
    }

    [Header("Scene Anchors")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private SeatingManager seatingManager;

    [Header("Timings")]
    [SerializeField] private float orderDuration = 2f;     
    [SerializeField] private float sitDuration = 30f;      

    private NavMeshAgent agent;
    private State state = State.None;
    private Seat mySeat;

    private Vector3 queuedTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Init(QueueManager qm, SeatingManager sm, Transform exit)
    {
        queueManager = qm;
        seatingManager = sm;
        exitPoint = exit;
        StartCoroutine(StateMachine());
    }

    // QueueManager uses this to reposition the customer within queue
    public void SetQueueTarget(Vector3 pos)
    {
        queuedTarget = pos;
        if (state == State.Queueing)
            MoveTo(queuedTarget);
    }

    public void OnDequeuedFromFront(Vector3 counterPos)
    {
        if (state == State.Queueing)
        {
            state = State.MovingToCounter;
            MoveTo(counterPos);
        }
    }

    private IEnumerator StateMachine()
    {
        state = State.Spawning;

        // Try to join the queue
        if (queueManager.TryJoinQueue(this))
        {
            state = State.Queueing;
            MoveTo(queuedTarget);

            // Wait in queue until we become the front and get dequeued (QueueManager will call OnDequeuedFromFront)
            while (state == State.Queueing)
                yield return null;

            // MovingToCounter handled when dequeued
            while (state == State.MovingToCounter)
            {
                if (ReachedDestination()) state = State.Ordering;
                yield return null;
            }
        }
        else
        {
            // Queue full → leave immediately
            state = State.Leaving;
        }

        // Ordering (placeholder)
        if (state == State.Ordering)
        {
            yield return new WaitForSeconds(orderDuration);
            queueManager.FrontCustomerDone();

            // Find a seat
            state = State.FindingSeat;
        }

        if (state == State.FindingSeat)
        {
            if (seatingManager.TryGetFreeSeat(out mySeat))
            {
                state = State.MovingToSeat;
                MoveTo(mySeat.transform.position);
            }
            else
            {
                // No seat available → just leave
                state = State.Leaving;
            }
        }

        while (state == State.MovingToSeat)
        {
            if (ReachedDestination())
            {
                state = State.Sitting;
            }
            yield return null;
        }

        if (state == State.Sitting)
        {
            // (Optional Later on) Attach a drink prop, etc.
            yield return new WaitForSeconds(sitDuration);
            if (mySeat != null)
            {
                seatingManager.ReleaseSeat(mySeat);
                mySeat = null;
            }
            state = State.Leaving;
        }

        if (state == State.Leaving)
        {
            MoveTo(exitPoint.position);
            while (!ReachedDestination())
                yield return null;

            Destroy(gameObject);
        }
    }

    private void MoveTo(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh)
            agent.SetDestination(pos);
        else
            Debug.LogWarning("NavMeshAgent missing or not on NavMesh.");
    }

    private bool ReachedDestination()
    {
        if (agent == null) return true;
        if (agent.pathPending) return false;
        if (agent.remainingDistance > agent.stoppingDistance) return false;
        if (agent.hasPath && agent.velocity.sqrMagnitude != 0f) return false;
        return true;
    }

    private void OnDestroy()
    {
        // if we were in queue and destroyed early, ensure queue is consistent
        if (queueManager != null && state == State.Queueing)
            queueManager.LeaveQueue(this);

        if (mySeat != null && seatingManager != null)
            seatingManager.ReleaseSeat(mySeat);
    }
}

