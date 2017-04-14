using System.Collections.Generic;

namespace TrainReservation.Domain
{
    public class Train
    {
        private readonly List<SeatWithBookingReference> seatWithBookingReferences;
        public string TrainId { get; }
        
        public Train(string trainId, List<SeatWithBookingReference> seatWithBookingReferences)
        {
            this.seatWithBookingReferences = seatWithBookingReferences;
            TrainId = trainId;
        }

        public ReservationOption Reserve(int requestSeatCount)
        {
            var option = new ReservationOption(this.TrainId, requestSeatCount);
            foreach (var seat in this.seatWithBookingReferences)
            {
                if (seat.IsAvailable())
                {
                    option.AddSeatReservation(seat.Seat);
                    if (option.Fullfiled)
                    {
                        break;
                    }
                }
            }

            return option;
        }
    }
}