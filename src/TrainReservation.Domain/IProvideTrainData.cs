using System.Collections.Generic;
using KataTrainReservation;
using Value;

namespace TrainReservation.Domain
{
    public interface IProvideTrainData
    {
        List<SeatWithBookingReference> GetSeats(string trainId);
        void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, List<Seat> seats);
    }

    public class SeatWithBookingReference
    {
        public Seat Seat { get; }
        public BookingReference BookingReference { get; }

        public SeatWithBookingReference(Seat seat, BookingReference bookingReference)
        {
            Seat = seat;
            BookingReference = bookingReference;
        }

        public bool IsAvailable()
        {
            return BookingReference.Equals(BookingReference.Null);
        }
    }

    public class BookingReference : ValueType<BookingReference>
    {
        private readonly string bookingReference;
        public string Value { get { return this.bookingReference; } }

        private static readonly BookingReference nullReference = new BookingReference(string.Empty);

        public BookingReference(string bookingReference)
        {
            this.bookingReference = bookingReference;
        }

        public static BookingReference Null { get { return nullReference; } }

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new[] {bookingReference};
        }

        public bool IsNull()
        {
            return Equals(Null);
        }
    }
}