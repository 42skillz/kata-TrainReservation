using System.Collections.Generic;
using Value;

namespace TrainReservation.Domain
{
    public class ReservationRequest : ValueType<ReservationRequest>
    {
        public string TrainId { get; }
        public int SeatCount { get; }

        public ReservationRequest(string trainId, int seatCount)
        {
            TrainId = trainId;
            SeatCount = seatCount;
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {TrainId, SeatCount};
        }
    }
}