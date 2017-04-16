using System.Collections.Generic;
using Value;

namespace TrainReservation.Domain
{
    public class SeatWithBookingReference : ValueType<SeatWithBookingReference>
    {
        public Seat Seat { get; }
        public BookingReference BookingReference { get; }

        public SeatWithBookingReference(Seat seat, BookingReference bookingReference)
        {
            Seat = seat;
            BookingReference = bookingReference;
        }

        public bool IsAvailable { get { return BookingReference.Equals(BookingReference.Null); } }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] { this.Seat, this.BookingReference };
        }

        public override string ToString()
        {
            return $"[{Seat}-{BookingReference}]";
        }
    }
}