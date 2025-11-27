using System.Collections.Generic;
using UnityEngine;

public class SeatingManager : MonoBehaviour
{
    [SerializeField] private List<Seat> seats = new List<Seat>();

    public bool TryGetFreeSeat(out Seat seat)
    {
        foreach (var s in seats)
        {
            if (!s.IsOccupied)
            {
                seat = s;
                s.Occupy();
                return true;
            }
        }
        seat = null;
        return false;
    }

    public void ReleaseSeat(Seat seat)
    {
        if (seat != null) seat.Vacate();
    }
}
