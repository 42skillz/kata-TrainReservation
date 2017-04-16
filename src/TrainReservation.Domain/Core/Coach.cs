using System;
using System.Collections.Generic;
using System.Linq;
using Value;

namespace TrainReservation.Domain.Core
{
    public class Coach : ValueType<Coach>
    {
        private const double SeventyPercent = 0.70d;
        private readonly string trainId;

        public Coach(string trainId, List<SeatWithBookingReference> seats)
        {
            this.trainId = trainId;
            Seats = seats;
        }

        public List<SeatWithBookingReference> Seats { get; }

        public int OverallCoachCapacity => Seats.Count;

        public int MaxReservableSeatsFollowingThePolicy => (int) Math.Round(OverallCoachCapacity * SeventyPercent);

        public int AlreadyReservedSeatsCount
        {
            get
            {
                return (from seat in Seats
                    where seat.IsAvailable == false
                    select seat).Count();
            }
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {this.trainId, new ListByValue<SeatWithBookingReference>(this.Seats) };
        }

        public bool HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(int requestedSeatCount)
        {
            return AlreadyReservedSeatsCount + requestedSeatCount <= MaxReservableSeatsFollowingThePolicy;
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            var option = new ReservationOption(trainId, requestedSeatCount);

            foreach (var seatWithBookingReference in Seats)
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