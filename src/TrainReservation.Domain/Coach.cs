using System.Collections.Generic;

namespace TrainReservation.Domain
{
    public class Coach
    {
        public string CoachId { get; }
        public List<SeatWithBookingReference> Seats { get; }

        public Coach(string coachId, List<SeatWithBookingReference> seats)
        {
            CoachId = coachId;
            Seats = seats;
        }
    }
}