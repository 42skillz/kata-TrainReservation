namespace TrainReservation.Domain
{
    public class SeatWithBookingReference
    {
        public Seat Seat { get; }
        public BookingReference BookingReference { get; }

        public SeatWithBookingReference(Seat seat, BookingReference bookingReference)
        {
            Seat = seat;
            BookingReference = bookingReference;
        }

        public bool IsAvailable { get { return BookingReference.Equals(BookingReference.Null); } }

        public override string ToString()
        {
            return $"[{Seat}-{BookingReference}]";
        }
    }
}