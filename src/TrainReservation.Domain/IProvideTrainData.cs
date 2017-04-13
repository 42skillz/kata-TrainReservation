using System.Collections.Generic;
using KataTrainReservation;

namespace TrainReservation.Domain
{
    public interface IProvideTrainData
    {
        List<SeatWithBookingReference> GetSeats(string trainId);
        void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, List<Seat> seats);
    }
}