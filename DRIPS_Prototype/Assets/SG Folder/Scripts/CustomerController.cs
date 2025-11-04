using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    public enum State
    {
        None, Spawning, Queueing, MovingToCounter, Ordering, FindingSeat,
        MovingToSeatApproach, TeleportToSeat, MovingToSeatDirect, Sitting, Leaving
    }

    [Header("Scene Anchors")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private SeatingManager seatingManager;

    [Header("Timings")]
    [SerializeField] private float orderDuration = 3f;   // <- 3 seconds at Slot_0
    [SerializeField] private float sitDuration = 30f;

    [Header("Arrival / Robustness")]
    [SerializeField] private float arriveDistance = 0.5f; // looser = safer
    [SerializeField] private float stuckTimeout = 4f;     // force progress

    private NavMeshAgent agent;
    private State state = State.None;
    private Seat mySeat;
    private Vector3 queuedTarget;
    private float stateEnterTime;

    // --------- Lifecycle ---------
    private void Awake() { agent = GetComponent<NavMeshAgent>(); }

    public void Init(QueueManager qm, SeatingManager sm, Transform exit)
    {
        queueManager = qm;
        seatingManager = sm;
        exitPoint = exit;
        StartCoroutine(StateMachine());
    }

    // --------- Called by QueueManager ---------
    public void SetQueueTarget(Vector3 pos)
    {
        queuedTarget = pos;
        if (state == State.Queueing) MoveTo(queuedTarget);
    }

    public void OnGoToCounter(Vector3 orderingPos)
    {
        if (state == State.Queueing || state == State.Spawning)
        {
            SetState(State.MovingToCounter);
            MoveTo(orderingPos);
        }
    }

    // --------- FSM ---------
    private IEnumerator StateMachine()
    {
        SetState(State.Spawning);

        // Try to join queue; if full -> leave
        if (queueManager.TryJoinQueue(this))
        {
            SetState(State.Queueing);
            MoveTo(queuedTarget);

            // wait until promoted to counter
            while (state == State.Queueing) yield return null;

            // walk to Slot_0; start ordering when close OR after timeout
            while (state == State.MovingToCounter)
            {
                if (Arrived() || TimedOut())
                    SetState(State.Ordering);
                yield return null;
            }
        }
        else
        {
            SetState(State.Leaving);
        }

        // ORDERING (guaranteed to progress after orderDuration)
        if (state == State.Ordering)
        {
            yield return new WaitForSeconds(orderDuration);
            if (queueManager != null) queueManager.CounterFreed();   // free Slot_0 for next
            SetState(State.FindingSeat);
        }

        // FIND SEAT
        if (state == State.FindingSeat)
        {
            if (seatingManager != null && seatingManager.TryGetFreeSeat(out mySeat) && mySeat != null)
            {
                // If seat has "Approach", go there first; else go straight to seat
                Transform approach = mySeat.transform.Find("Approach");
                if (approach != null)
                {
                    SetState(State.MovingToSeatApproach);
                    MoveTo(approach.position);
                }
                else
                {
                    SetState(State.MovingToSeatDirect);
                    MoveTo(mySeat.transform.position);
                }
            }
            else
            {
                SetState(State.Leaving); // no seats
            }
        }

        // MOVE TO APPROACH
        while (state == State.MovingToSeatApproach)
        {
            if (Arrived() || TimedOut())
                SetState(State.TeleportToSeat);
            yield return null;
        }

        // TELEPORT TO "SitPoint" (or seat transform if missing)
        if (state == State.TeleportToSeat)
        {
            Transform sitPoint = mySeat != null ? mySeat.transform.Find("SitPoint") : null;
            Vector3 sitPos = sitPoint ? sitPoint.position : (mySeat ? mySeat.transform.position : transform.position);
            Quaternion sitRot = sitPoint ? sitPoint.rotation : (mySeat ? mySeat.transform.rotation : transform.rotation);
            SafeWarp(sitPos);
            transform.rotation = sitRot;
            if (agent) agent.isStopped = true;
            SetState(State.Sitting);
        }

        // MOVE DIRECTLY TO SEAT
        while (state == State.MovingToSeatDirect)
        {
            if (Arrived() || TimedOut())
                SetState(State.Sitting);
            yield return null;
        }

        // SIT, THEN LEAVE
        if (state == State.Sitting)
        {
            yield return new WaitForSeconds(sitDuration);
            if (mySeat != null && seatingManager != null) seatingManager.ReleaseSeat(mySeat);
            mySeat = null;
            if (agent) agent.isStopped = false;
            SetState(State.Leaving);
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

    // --------- Helpers ---------
    private void MoveTo(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh) agent.SetDestination(pos);
    }

    private bool Arrived()
    {
        if (agent == null) return true;
        if (agent.pathPending) return false;
        if (agent.remainingDistance <= Mathf.Max(agent.stoppingDistance, arriveDistance)) return true;
        if (!agent.hasPath && agent.velocity.sqrMagnitude < 0.01f)
            return Vector3.Distance(transform.position, agent.destination) <= arriveDistance;
        return false;
    }

    private bool TimedOut() => Time.time - stateEnterTime > stuckTimeout;

    private void SetState(State s)
    {
        state = s;
        stateEnterTime = Time.time;
        // Debug breadcrumb – watch this in Console to verify progress:
        Debug.Log($"{name} -> {state}");
    }

    private void SafeWarp(Vector3 position)
    {
        if (agent != null) agent.Warp(position);
        else transform.position = position;
    }

    private void OnDestroy()
    {
        if (queueManager != null && state == State.Queueing) queueManager.LeaveQueue(this);
        if (mySeat != null && seatingManager != null) seatingManager.ReleaseSeat(mySeat);
    }
}
