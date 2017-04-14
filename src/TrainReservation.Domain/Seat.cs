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
            return this.seats.GetEnumerator();
        }

        public IEnumerator<Seat> GetEnumerator()
        {
            return this.seats.GetEnumerator();
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new List<object>() { new ListByValue<Seat>(this.seats) };
        }

        public void Add(Seat seat)
        {
            this.seats.Add(seat);
        }

        public int Count => this.seats.Count;
    }

    public class Seat
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
            return $"{this.Coach}{this.SeatNumber}";
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// N.B. this is not how you would override equals in a production environment. :)
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as Seat;

            return Coach == other.Coach && SeatNumber == other.SeatNumber;
        }
    }
}