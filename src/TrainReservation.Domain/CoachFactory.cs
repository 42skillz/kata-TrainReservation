using System;
using System.Linq;
using System.Collections.Generic;

namespace TrainReservation.Domain
{
    public class CoachFactory
    {
        public static Dictionary<string, Coach> InstantiateCoaches(string trainId, IEnumerable<SeatWithBookingReference> seatsWithBookingReferences)
        {
            var result =  new Dictionary<string, Coach>();

            var coachNames = (from sbr in seatsWithBookingReferences
                             select sbr.Seat.Coach).Distinct();

            foreach (var coachName in coachNames)
            {
                var seatsForThisCoach = from sbr in seatsWithBookingReferences
                    where sbr.Seat.Coach == coachName
                    select sbr;

                var coach = new Coach(trainId, coachName, new List<SeatWithBookingReference>(seatsForThisCoach));
                result[coachName] = coach;
            }

            return result;
        }
    }
}