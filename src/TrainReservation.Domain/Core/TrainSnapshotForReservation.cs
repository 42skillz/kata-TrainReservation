using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace TrainReservation.Domain.Core
{
    /// <summary>
    /// Aggregate allowing seat allocation during reservations following the business rules defined with the domain experts. 
    /// </summary>
    public class TrainSnapshotForReservation
    {
        private const double SeventyPercent = 0.70d;
        private readonly Dictionary<string, Coach> coaches;

        public string TrainId { get; }

        public TrainSnapshotForReservation(string trainId, IEnumerable<SeatWithBookingReference> seatsWithBookingReferences)
        {
            this.TrainId = trainId;

            this.SeatsWithBookingReferences = seatsWithBookingReferences;
            this.coaches = CoachFactory.InstantiateCoaches(trainId, seatsWithBookingReferences);
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

        #region Traits in common with Coach?

        public int OverallTrainCapacity
        {
            get
            {
                return this.coaches.Values.Sum(coach => coach.OverallCoachCapacity);
            }   
        }

        public int MaxReservableSeatsFollowingThePolicy => (int) Math.Round(OverallTrainCapacity * SeventyPercent);

        public int AlreadyReservedSeatsCount
        {
            get
            {
                return this.coaches.Values.Sum(coach => coach.AlreadyReservedSeatsCount);
            }
        }
        
        #endregion

        public IEnumerable<SeatWithBookingReference> SeatsWithBookingReferences { get; }

        public int CoachCount => this.coaches.Count;
    }
}