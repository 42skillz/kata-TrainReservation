using System.Collections.Generic;

namespace TrainReservation.Domain
{
    public class Coach
    {
        private readonly string trainId;
        public string CoachId { get; }
        public List<SeatWithBookingReference> Seats { get; }

        public Coach(string trainId, string coachId, List<SeatWithBookingReference> seats)
        {
            this.trainId = trainId;
            this.CoachId = coachId;
            this.Seats = seats;
        }

        public bool HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(int requestedSeatCount)
        {
            throw new System.NotImplementedException();
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            //foreach (var seat in SeatsWithBookingReferences)
            //{
            //    if (seat.IsAvailable)
            //    {
            //        option.AddSeatReservation(seat.Seat);
            //        if (option.IsFullfiled)
            //        {
            //            break;
            //        }
            //    }
            //}
            throw new System.NotImplementedException();
        }
    }
}