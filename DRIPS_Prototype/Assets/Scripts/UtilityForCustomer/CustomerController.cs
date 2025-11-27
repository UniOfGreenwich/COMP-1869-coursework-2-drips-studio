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
        MovingToSeat,   // walk to Approach or Seat
        Sitting,
        Leaving
    }

    [Header("Refs")]
    [SerializeField] private Transform exitPoint;
    [SerializeField] private QueueManager queueManager;
    [SerializeField] private SeatingManager seatingManager;

    [Header("Timings")]
    [SerializeField] private float sitDuration = 30f;

    [Header("Arrival")]
    [SerializeField] private float arriveDistance = 0.6f;
    [SerializeField] public bool hasBeenServed = false;

    private TrashCanStation trashCanStation;
    private NavMeshAgent agent;
    private State state;
    private Vector3 queueTarget;
    private Seat mySeat;

    private void Awake() => agent = GetComponent<NavMeshAgent>();

    private void Start()
    {
        trashCanStation = FindAnyObjectByType<TrashCanStation>();
    }

    public void Init(QueueManager qm, SeatingManager sm, Transform exit)
    {
        queueManager = qm;
        seatingManager = sm;
        exitPoint = exit;
        StartCoroutine(Main());
    }

    // Called by QueueManager
    public void SetQueueTarget(Vector3 pos)
    {
        queueTarget = pos;
        if (state != State.Queueing) state = State.Queueing;
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
            state = State.Leaving;

        // Wait in queue until promoted
        while (state == State.Queueing) yield return null;

        // Move to counter and wait until we’ve arrived
        while (state == State.MovingToCounter && !Arrived())
            yield return null;

        // Order
        state = State.Ordering;
        yield return new WaitUntil(() => hasBeenServed == true);
        queueManager.CounterFreed();

        // Find a seat
        state = State.FindingSeat;
        if (seatingManager.TryGetFreeSeat(out mySeat) && mySeat != null)
        {
            // ---- WALK TO APPROACH (if present) OR TO SEAT ----

            if(mySeat.approachPointLeft || mySeat.approachPointRight)
            {
                Vector3 walkTarget;
                float distanceFromLeft = Vector3.Distance(gameObject.transform.position, mySeat.approachPointLeft.position);
                float distanceFromRight = Vector3.Distance(gameObject.transform.position, mySeat.approachPointRight.position);

                if (distanceFromLeft < distanceFromRight)
                {
                    walkTarget = mySeat.approachPointLeft.position;
                }
                else if (distanceFromRight < distanceFromLeft)
                {
                    walkTarget = mySeat.approachPointRight.position;
                }
                else
                {
                    walkTarget = mySeat.transform.position;
                }

                state = State.MovingToSeat;
                MoveTo(walkTarget);
            }

            while (!Arrived()) yield return null;

            // ---- TELEPORT ONTO THE CHAIR ----
            Vector3 sitPos = mySeat.sitPoint ? mySeat.sitPoint.position : mySeat.transform.position;
            Quaternion sitRot = mySeat.sitPoint ? mySeat.sitPoint.rotation : mySeat.transform.rotation;

            SafeWarp(sitPos);
            transform.rotation = sitRot;
            if (agent) agent.isStopped = true;

            // ---- SIT, THEN LEAVE ----
            state = State.Sitting;
            yield return new WaitForSeconds(sitDuration);

            if (agent) agent.isStopped = false;
            seatingManager.ReleaseSeat(mySeat);
            mySeat = null;

            // +1 to trash can
            trashCanStation.IncreaseBinLevel();
        }

        // Leave
        state = State.Leaving;
        MoveTo(exitPoint.position);
        yield return new WaitForSeconds(5f); // simple exit window
        Destroy(gameObject);
    }

    private void MoveTo(Vector3 pos)
    {
        if (agent != null && agent.isOnNavMesh)
            agent.SetDestination(pos);
    }

    private bool Arrived()
    {
        if (agent == null || agent.pathPending) return false;
        return agent.remainingDistance <= Mathf.Max(arriveDistance, agent.stoppingDistance);
    }

    private void SafeWarp(Vector3 position)
    {
        if (agent != null) agent.Warp(position);
        else transform.position = position;
    }

    private void OnDestroy()
    {
        if (mySeat != null && seatingManager != null)
            seatingManager.ReleaseSeat(mySeat);
    }

}


