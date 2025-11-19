using UnityEngine;

public class Seat : MonoBehaviour
{
    [Header("Waypoints (optional but recommended)")]
    [Tooltip("Point on the NavMesh in front of the chair.")]
    public Transform approachPointLeft;
    public Transform approachPointRight;

    [Tooltip("Exact seat position (can be off the NavMesh).")]
    public Transform sitPoint;

    public bool IsOccupied { get; private set; }

    public void Occupy() => IsOccupied = true;
    public void Vacate() => IsOccupied = false;

    private void OnValidate()
    {
        // Auto-find children called "Approach" or "SitPoint" if not assigned
        if (approachPointLeft == null && approachPointRight == null)
        {
            var tl = transform.Find("ApproachLeft");
            if (tl) approachPointLeft = tl;

            var tr = transform.Find("ApproachRight");
            if (tr) approachPointRight = tr;
        }

        if (sitPoint == null)
        {
            var t = transform.Find("Sit");
            if (t) sitPoint = t;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.15f);

        if (approachPointLeft && approachPointRight)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(approachPointLeft.position, 0.1f);
            Gizmos.DrawLine(transform.position, approachPointLeft.position);
            Gizmos.DrawWireSphere(approachPointRight.position, 0.1f);
            Gizmos.DrawLine(transform.position, approachPointRight.position);
        }

        if (sitPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(sitPoint.position, Vector3.one * 0.1f);
        }
    }
}
