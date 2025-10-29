using UnityEngine;

public class Seat : MonoBehaviour
{
    public bool IsOccupied { get; private set; }

    public void Occupy() { IsOccupied = true; }
    public void Vacate() { IsOccupied = false; }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
}
