using System;
using System.Collections.Generic;
using System.Linq;
using Value;

namespace TrainReservation.Domain.Core
{
    /// <summary>
    /// Aggregate allowing seat allocation during reservations following the business rules defined with the domain experts. 
    /// </summary>
    public class TrainSnapshotForReservation : ValueType<TrainSnapshotForReservation>
    {
        private const double SeventyPercent = 0.70d;
        private readonly Dictionary<string, Coach> coaches;
        private readonly List<SeatWithBookingReference> seatsWithBookingReferences;

        public string TrainId { get; }

        public TrainSnapshotForReservation(string trainId, IEnumerable<SeatWithBookingReference> seatsWithBookingReferences)
        {
            this.TrainId = trainId;

            this.seatsWithBookingReferences = new List<SeatWithBookingReference>(seatsWithBookingReferences);

            this.coaches = CoachFactory.InstantiateCoaches(trainId, seatsWithBookingReferences);
        }

        public int OverallTrainCapacity => this.coaches.Values.Sum(coach => coach.OverallCoachCapacity);

        public int MaxReservableSeatsFollowingThePolicy => (int)Math.Round(OverallTrainCapacity * SeventyPercent);

        public int AlreadyReservedSeatsCount
        {
            get
            {
                return this.coaches.Values.Sum(coach => coach.AlreadyReservedSeatsCount);
            }
        }

        public IEnumerable<SeatWithBookingReference> SeatsWithBookingReferences => this.seatsWithBookingReferences;

        public int CoachCount => this.coaches.Count;

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] { this.TrainId, new ListByValue<SeatWithBookingReference>(this.seatsWithBookingReferences) };
        }

        public ReservationOption Reserve(int requestedSeatCount)
        {
            var option = new ReservationOption(TrainId, requestedSeatCount);

            if ((AlreadyReservedSeatsCount + requestedSeatCount) > MaxReservableSeatsFollowingThePolicy)
            {
                return option;
            }

            // Try the nice way (i.e. by respecting the "no more 70% of every coach" rule)
            foreach (var coach in this.coaches.Values)
            {
                if (coach.HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(requestedSeatCount))
                {
                    option = coach.Reserve(requestedSeatCount);
                    if (option.IsFullfiled)
                    {
                        return option;
                    }
                }
            }

            if (!option.IsFullfiled)
            {
                // Try the hard way (i.e. don't respect the "no more 70% of every coach" rule)
                foreach (var coach in this.coaches.Values)
                {
                    option = coach.Reserve(requestedSeatCount);
                    if (option.IsFullfiled)
                    {
                        return option;
                    }
                }
            }
            
            return option;
        }
    }
}