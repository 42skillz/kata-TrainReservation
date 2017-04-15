using System;
using System.Collections.Generic;

namespace TrainReservation.Domain
{
    public class Train
    {
        private const double SeventyPercent = 0.70d;
        private readonly List<SeatWithBookingReference> seatsWithBookingReferences;
        public string TrainId { get; }

        public Train(string trainId, List<SeatWithBookingReference> seatsWithBookingReferences)
        {
            this.seatsWithBookingReferences = seatsWithBookingReferences;
            TrainId = trainId;
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            var option = new ReservationOption(TrainId, requestedSeatCount);

            if (AvailableSeatsCount < requestedSeatCount)
            {
                return option;
            }

            if (AvailableSeatsCount - requestedSeatCount > MaxReservableSeatsFollowingThePolicy)
            {
                return option;
            }

            foreach (var seat in seatsWithBookingReferences)
            {
                if (seat.IsAvailable)
                {
                    option.AddSeatReservation(seat.Seat);
                    if (option.IsFullfiled)
                    {
                        break;
                    }
                }
            }

            return option;
        }

        public int OverallTrainCapacity { get { return seatsWithBookingReferences.Count; } }

        public int MaxReservableSeatsFollowingThePolicy => (int)Math.Round(OverallTrainCapacity * SeventyPercent);

        public int AvailableSeatsCount
        {
            get
            {
                var availableSeatsCount = 0;
                foreach (var seatWithBookingReference in seatsWithBookingReferences)
                {
                    if (seatWithBookingReference.IsAvailable)
                    {
                        availableSeatsCount++;
                    }
                }

                return availableSeatsCount;
            }
        }
    }
}