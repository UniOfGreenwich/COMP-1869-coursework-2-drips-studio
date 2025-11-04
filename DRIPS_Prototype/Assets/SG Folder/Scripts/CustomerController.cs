using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    public enum State
    {
        None,
        Spawning,
        Queueing,
        MovingToCounter,   // walk to Slot_0 (order spot)
        Ordering,          // wait orderDuration
        FindingSeat,
        MovingToSeatApproach,
        TeleportToSeat,
        MovingToSeatDirect,
        Sitting,
        Leaving
    }

    [Header("Scene Anchors")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private SeatingManager seatingManager;

    [Header("Timings")]
    [SerializeField] private float orderDuration = 3f;   // time at Slot_0
    [SerializeField] private float sitDuration = 30f;    // time seated

    [Header("Arrival / Robustness")]
    [SerializeField] private float arriveDistance = 0.6f;
    [SerializeField] private float stuckTimeout = 2f;
    [SerializeField] private float forceOrderingDelay = 1.2f; // safety: start ordering shortly after promotion

    private NavMeshAgent agent;
    private State state = State.None;
    private Seat mySeat;
    private Vector3 queuedTarget;
    private float stateEnterTime;

    // ---------- Lifecycle ----------
    private void Awake() => agent = GetComponent<NavMeshAgent>();

    public void Init(QueueManager qm, SeatingManager sm, Transform exit)
    {
        queueManager = qm;
        seatingManager = sm;
        exitPoint = exit;
        StartCoroutine(StateMachine());
    }

    // ---------- Called by QueueManager ----------
    public void SetQueueTarget(Vector3 pos)
    {
        queuedTarget = pos;
        if (state == State.Queueing) MoveTo(queuedTarget);
    }

    public void OnGoToCounter(Vector3 orderingPos)
    {
        if (state == State.Queueing || state == State.Spawning)
        {
            state = State.MovingToCounter;
            MoveTo(orderingPos);
            MarkStateEnter();
            // Safety: even if arrival is finicky, start ordering after a short delay
            StartCoroutine(ForceOrderingAfter(forceOrderingDelay));
        }
    }

    // ---------- FSM ----------
    private IEnumerator StateMachine()
    {
        state = State.Spawning;

        // Try to join queue; if full, leave
        if (queueManager.TryJoinQueue(this))
        {
            state = State.Queueing;
            MoveTo(queuedTarget);
            MarkStateEnter();

            // Wait until promoted
            while (state == State.Queueing) yield return null;

            // Move to Slot_0; advance on arrival or timeout (plus the safety above)
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

        // ORDERING at Slot_0
        if (state == State.Ordering)
        {
            yield return new WaitForSeconds(orderDuration);

            // free Slot_0 for next customer
            if (queueManager != null) queueManager.CounterFreed();

            // === GO FIND A SEAT ===
            state = State.FindingSeat;
        }

        // FIND A SEAT
        if (state == State.FindingSeat)
        {
            if (seatingManager != null && seatingManager.TryGetFreeSeat(out mySeat) && mySeat != null)
            {
                // If seat has "Approach" child, go there; else go straight to seat
                Transform approach = mySeat.transform.Find("Approach");
                if (approach != null)
                {
                    state = State.MovingToSeatApproach;
                    MoveTo(approach.position);
                    MarkStateEnter();
                }
                else
                {
                    state = State.MovingToSeatDirect;
                    MoveTo(mySeat.transform.position);
                    MarkStateEnter();
                }
            }
            else
            {
                // no seats → leave right away
                state = State.Leaving;
            }
        }

        // MOVE TO APPROACH
        while (state == State.MovingToSeatApproach)
        {
            if (Arrived() || TimedOut())
                state = State.TeleportToSeat;
            yield return null;
        }

        // TELEPORT TO SitPoint (or seat transform if none)
        if (state == State.TeleportToSeat)
        {
            Transform sitPoint = mySeat != null ? mySeat.transform.Find("SitPoint") : null;
            Vector3 sitPos = sitPoint ? sitPoint.position : (mySeat ? mySeat.transform.position : transform.position);
            Quaternion sitRot = sitPoint ? sitPoint.rotation : (mySeat ? mySeat.transform.rotation : transform.rotation);

            SafeWarp(sitPos);
            transform.rotation = sitRot;

            if (agent) agent.isStopped = true;

            state = State.Sitting;
            MarkStateEnter();
        }

        // MOVE DIRECTLY TO SEAT (no Approach)
        while (state == State.MovingToSeatDirect)
        {
            if (Arrived() || TimedOut())
                state = State.Sitting;
            yield return null;
        }

        // SIT, THEN LEAVE
        if (state == State.Sitting)
        {
            yield return new WaitForSeconds(sitDuration);

            if (mySeat != null && seatingManager != null)
            {
                seatingManager.ReleaseSeat(mySeat);
                mySeat = null;
            }

            if (agent) agent.isStopped = false;

            state = State.Leaving;
        }

        // LEAVE
        if (state == State.Leaving)
        {
            MoveTo(exitPoint.position);
            float t0 = Time.time;
            while (!Arrived() && Time.time - t0 < 12f) yield return null;
            Destroy(gameObject);
        }
    }

    // ---------- Helpers ----------
    private IEnumerator ForceOrderingAfter(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (state == State.MovingToCounter) state = State.Ordering;
    }

    private void MoveTo(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh) agent.SetDestination(pos);
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

    private void SafeWarp(Vector3 position)
    {
        if (agent != null) agent.Warp(position);
        else transform.position = position;
    }

    private void OnDestroy()
    {
        if (queueManager != null && state == State.Queueing)
            queueManager.LeaveQueue(this);

        if (mySeat != null && seatingManager != null)
            seatingManager.ReleaseSeat(mySeat);
    }
}
