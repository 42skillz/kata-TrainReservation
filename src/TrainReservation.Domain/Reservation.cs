using System.Collections.Generic;
using KataTrainReservation;

namespace TrainReservation
{
    public class Reservation
    {
        public string TrainId { get; }
        public string BookingId { get; }
        public List<Seat> Seats { get; }

        public Reservation(string trainId, string bookingId, List<Seat> seats)
        {
            TrainId = trainId;
            BookingId = bookingId;
            Seats = seats;
        }
    }
}