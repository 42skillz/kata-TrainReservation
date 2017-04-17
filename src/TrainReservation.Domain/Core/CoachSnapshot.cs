using System;
using System.Collections.Generic;
using System.Linq;
using Value;

namespace TrainReservation.Domain.Core
{
    /// <summary>
    /// Snapshot of a coach topology allowing to find available seats following the business rules defined with the domain experts. 
    /// </summary>
    public class CoachSnapshot : ValueType<CoachSnapshot>
    {
        private const double SeventyPercent = 0.70d;
        private readonly string trainId;

        public CoachSnapshot(string trainId, List<SeatWithBookingReference> seats)
        {
            this.trainId = trainId;
            this.Seats = seats;
        }

        public List<SeatWithBookingReference> Seats { get; }

        public int OverallCoachCapacity => this.Seats.Count;

        public int MaxReservableSeatsFollowingThePolicy => (int) Math.Round(OverallCoachCapacity * SeventyPercent);

        public int AlreadyReservedSeatsCount
        {
            get
            {
                return (from seat in this.Seats
                    where seat.IsAvailable == false
                    select seat).Count();
            }
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] { this.trainId, new ListByValue<SeatWithBookingReference>(this.Seats)};
        }

        public bool HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(int requestedSeatCount)
        {
            return AlreadyReservedSeatsCount + requestedSeatCount <= MaxReservableSeatsFollowingThePolicy;
        }

        public ReservationOption FindReservationOption(int requestedSeatCount)
        {
            var option = new ReservationOption(trainId, requestedSeatCount);

            foreach (var seatWithBookingReference in this.Seats)
            {
                if (seatWithBookingReference.IsAvailable)
                {
                    option.AddSeatReservation(seatWithBookingReference.Seat);
                    if (option.IsFullfiled)
                    {
                        break;
                    }
                }
            }

            return option;
        }
    }
}