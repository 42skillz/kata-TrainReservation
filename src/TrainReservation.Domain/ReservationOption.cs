namespace TrainReservation.Domain
{
    public class ReservationOption
    {
        private readonly string trainId;
        private readonly int requestSeatCount;
        private readonly Seats reservedSeats;
        public bool IsFullfiled { get; private set; }
        public Seats ReservedSeats { get { return reservedSeats; } }

        public ReservationOption(string trainId, int requestSeatCount)
        {
            this.trainId = trainId;
            this.requestSeatCount = requestSeatCount;
            reservedSeats = new Seats();
        }

        public void AddSeatReservation(Seat seat)
        {
            if (!IsFullfiled)
            {
                reservedSeats.Add(seat);
                MarkAsFullfiledWhenAllRequestedSeatsAreReserved();
            }
        }

        private void MarkAsFullfiledWhenAllRequestedSeatsAreReserved()
        {
            if (reservedSeats.Count == requestSeatCount)
            {
                IsFullfiled = true;
            }
        }
    }
}