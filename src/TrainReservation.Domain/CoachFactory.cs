using System;
using System.Collections.Generic;

namespace TrainReservation.Domain
{
    public class CoachFactory
    {
        public static Dictionary<string, Coach> InstantiateCoaches(IEnumerable<SeatWithBookingReference> seatsWithBookingReferences)
        {
            return new Dictionary<string, Coach>();
        }
    }
}