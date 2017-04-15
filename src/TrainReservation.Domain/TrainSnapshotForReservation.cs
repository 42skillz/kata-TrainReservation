using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainReservation.Domain
{
    /// <summary>
    /// Aggregate allowing seat allocation during reservations following the business rules defined with the domain experts. 
    /// </summary>
    public class TrainSnapshotForReservation
    {
        private const double SeventyPercent = 0.70d;
        private readonly IEnumerable<SeatWithBookingReference> seatsWithBookingReferences;
        public string TrainId { get; }

        public TrainSnapshotForReservation(string trainId, IEnumerable<SeatWithBookingReference> seatsWithBookingReferences)
        {
            this.seatsWithBookingReferences = seatsWithBookingReferences;
            TrainId = trainId;
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            var option = new ReservationOption(TrainId, requestedSeatCount);

            if (requestedSeatCount > AvailableSeatsCount)
            {
                return option;
            }

            if (requestedSeatCount > MaxReservableSeatsFollowingThePolicy)
            {
                return option;
            }

            foreach (var seat in SeatsWithBookingReferences)
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

        public int OverallTrainCapacity => SeatsWithBookingReferences.Count();

        public int MaxReservableSeatsFollowingThePolicy => (int) Math.Round(OverallTrainCapacity * SeventyPercent);

        public int AvailableSeatsCount
        {
            get
            {
                var availableSeatsCount = 0;
                foreach (var seatWithBookingReference in SeatsWithBookingReferences)
                {
                    if (seatWithBookingReference.IsAvailable)
                    {
                        availableSeatsCount++;
                    }
                }

                return availableSeatsCount;
            }
        }

        public IEnumerable<SeatWithBookingReference> SeatsWithBookingReferences
        {
            get { return seatsWithBookingReferences; }
        }
    }
}