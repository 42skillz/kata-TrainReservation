using System.Collections;
using System.Collections.Generic;
using Value;

namespace TrainReservation.Domain
{
    public class Seats : ValueType<Seats>, IEnumerable<Seat>
    {
        private readonly List<Seat> seats;

        public Seats() : this(new List<Seat>())
        {
        }

        public Seats(IEnumerable<Seat> seats)
        {
            this.seats = new List<Seat>(seats);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return seats.GetEnumerator();
        }

        public IEnumerator<Seat> GetEnumerator()
        {
            return seats.GetEnumerator();
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new List<object>() {new ListByValue<Seat>(seats)};
        }

        public void Add(Seat seat)
        {
            seats.Add(seat);
        }

        public int Count => seats.Count;
    }
}