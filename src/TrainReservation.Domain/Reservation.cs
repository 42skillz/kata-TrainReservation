using System.Collections.Generic;
using TrainReservation.Domain;
using Value;

namespace TrainReservation
{
    public class Reservation : ValueType<Reservation>
    {
        public string TrainId { get; }
        public BookingReference BookingReference { get; }
        public Seats Seats { get; }

        public Reservation(string trainId, BookingReference bookingReference, Seats seats)
        {
            TrainId = trainId;
            BookingReference = bookingReference;
            Seats = seats;
        }

        public Reservation(string trainId) : this(trainId, BookingReference.Null, new Seats())
        {
        }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new List<object>() {TrainId, BookingReference, Seats};
        }
    }
}