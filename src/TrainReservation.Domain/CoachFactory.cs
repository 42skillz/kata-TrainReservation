using System.Collections.Generic;
using System.Linq;
using TrainReservation.Domain.Core;

namespace TrainReservation.Domain
{
    public static class CoachFactory
    {
        public static Dictionary<string, CoachSnapshotForReservation> InstantiateCoaches(string trainId, IEnumerable<SeatWithBookingReference> seatsWithBookingReferences)
        {
            var result = new Dictionary<string, CoachSnapshotForReservation>();

            var coachNames = (from sbr in seatsWithBookingReferences
                select sbr.Seat.Coach).Distinct();

            foreach (var coachName in coachNames)
            {
                var seatsForThisCoach = from sbr in seatsWithBookingReferences
                    where sbr.Seat.Coach == coachName
                    select sbr;

                var coach = new CoachSnapshotForReservation(trainId, new List<SeatWithBookingReference>(seatsForThisCoach));
                result[coachName] = coach;
            }

            return result;
        }
    }
}