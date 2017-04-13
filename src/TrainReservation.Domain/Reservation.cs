using System.Collections.Generic;
using KataTrainReservation;
using TrainReservation.Domain;

namespace TrainReservation
{
    public class Reservation
    {
        public string TrainId { get; }
        public BookingReference BookingReference { get; }
        public List<Seat> Seats { get; }

        public Reservation(string trainId, string bookingReference, List<Seat> seats) : this(trainId, new BookingReference(bookingReference), seats)
        {
        }

        public Reservation(string trainId, BookingReference bookingReference, List<Seat> seats)
        {
            TrainId = trainId;
            BookingReference = bookingReference;
            Seats = seats;
        }
    }
}