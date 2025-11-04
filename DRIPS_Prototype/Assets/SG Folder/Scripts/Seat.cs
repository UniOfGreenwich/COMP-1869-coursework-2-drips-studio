using UnityEngine;

public class Seat : MonoBehaviour
{
    [Header("Waypoints (optional but recommended)")]
    [Tooltip("Point on the NavMesh in front of the chair.")]
    public Transform approachPoint;

    [Tooltip("Exact seat position (can be off the NavMesh).")]
    public Transform sitPoint;

    public bool IsOccupied { get; private set; }

    public void Occupy() => IsOccupied = true;
    public void Vacate() => IsOccupied = false;

    private void OnValidate()
    {
        // Auto-find children called "Approach" or "SitPoint" if not assigned
        if (approachPoint == null)
        {
            var t = transform.Find("Approach");
            if (t) approachPoint = t;
        }

        if (sitPoint == null)
        {
            var t = transform.Find("SitPoint");
            if (t) sitPoint = t;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = IsOccupied ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.15f);

        if (approachPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(approachPoint.position, 0.1f);
            Gizmos.DrawLine(transform.position, approachPoint.position);
        }

        if (sitPoint)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(sitPoint.position, Vector3.one * 0.1f);
        }
    }
}
