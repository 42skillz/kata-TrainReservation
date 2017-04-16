using System.Collections.Generic;
using Value;

namespace TrainReservation.Domain
{
    public class BookingReference : ValueType<BookingReference>
    {
        private readonly string bookingReference;
        public string Value { get { return bookingReference; } }

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

        public override string ToString()
        {
            return Value;
        }
    }
}