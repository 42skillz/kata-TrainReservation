using System.Collections.Generic;
using KataTrainReservation;

namespace TrainReservation.Domain
{
    public class ReservationOption
    {
        private readonly string trainId;
        private readonly int requestSeatCount;
        private readonly Seats reservedSeats;
        public bool Fullfiled { get; private set; }
        public Seats ReservedSeats { get { return this.reservedSeats; } }

        public ReservationOption(string trainId, int requestSeatCount)
        {
            this.trainId = trainId;
            this.requestSeatCount = requestSeatCount;
            this.reservedSeats = new Seats();
        }

        public void AddSeatReservation(Seat seat)
        {
            if (!this.Fullfiled)
            {
                this.reservedSeats.Add(seat);
                if (this.reservedSeats.Count == this.requestSeatCount)
                {
                    this.Fullfiled = true;
                }
            }
        }
    }
}