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

    [Header("Arrival Tuning")]
    [SerializeField] private float arriveDistance = 0.4f;
    [SerializeField] private float stuckTimeout = 6f;

    private NavMeshAgent agent;
    private State state = State.None;
    private Seat mySeat;
    private Vector3 queuedTarget;
    private float stateEnterTime;

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

    public void SetQueueTarget(Vector3 pos)
    {
        queuedTarget = pos;
        if (state == State.Queueing)
            MoveTo(queuedTarget);
    }

    // NEW: called when promoted to counter
    public void OnGoToCounter(Vector3 counterPos)
    {
        if (state == State.Queueing || state == State.Spawning)
        {
            state = State.MovingToCounter;
            MoveTo(counterPos);
            MarkStateEnter();
        }
    }

    private IEnumerator StateMachine()
    {
        state = State.Spawning;

        if (queueManager.TryJoinQueue(this))
        {
            state = State.Queueing;
            MoveTo(queuedTarget);
            MarkStateEnter();

            // Wait until QueueManager promotes us (calls OnGoToCounter)
            while (state == State.Queueing)
                yield return null;

            // Move to counter with robust arrival or timeout
            while (state == State.MovingToCounter)
            {
                if (Arrived() || TimedOut()) state = State.Ordering;
                yield return null;
            }
        }
        else
        {
            state = State.Leaving;
        }

        // Ordering
        if (state == State.Ordering)
        {
            yield return new WaitForSeconds(orderDuration);
            queueManager.CounterFreed();      // let next customer advance
            state = State.FindingSeat;
        }

        if (state == State.FindingSeat)
        {
            if (seatingManager.TryGetFreeSeat(out mySeat))
            {
                state = State.MovingToSeat;
                // If you added Approach/SitPoint later, you can change this to mySeat.approachPoint.position
                MoveTo(mySeat.transform.position);
                MarkStateEnter();
            }
            else
            {
                state = State.Leaving;
            }
        }

        while (state == State.MovingToSeat)
        {
            if (Arrived() || TimedOut())
                state = State.Sitting;
            yield return null;
        }

        if (state == State.Sitting)
        {
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
            float t0 = Time.time;
            while (!Arrived() && Time.time - t0 < 12f) yield return null;
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

    private bool Arrived()
    {
        if (agent == null) return true;
        if (agent.pathPending) return false;

        if (agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arriveDistance))
            return true;

        if (!agent.hasPath && agent.velocity.sqrMagnitude < 0.01f)
            return Vector3.Distance(transform.position, agent.destination) <= arriveDistance;

        return false;
    }

    private bool TimedOut() => Time.time - stateEnterTime > stuckTimeout;
    private void MarkStateEnter() => stateEnterTime = Time.time;

    private void OnDestroy()
    {
        if (queueManager != null && state == State.Queueing)
            queueManager.LeaveQueue(this);

        if (mySeat != null && seatingManager != null)
            seatingManager.ReleaseSeat(mySeat);
    }
}
