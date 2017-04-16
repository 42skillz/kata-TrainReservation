using System.Collections.Generic;
using Value;

namespace TrainReservation.Domain
{
    public class Seat : ValueType<Seat>
    {
        public string Coach { get; }
        public int SeatNumber { get; }

        public Seat(string coach, int seatNumber)
        {
            Coach = coach;
            SeatNumber = seatNumber;
        }

        public override string ToString()
        {
            return $"{Coach}{SeatNumber}";
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {Coach, SeatNumber};
        }
    }
}