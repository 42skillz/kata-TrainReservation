using TrainReservation.Domain.Core;

namespace TrainReservation.Domain
{
    public interface IProvideTrainData
    {
        // List<SeatWithBookingReference> GetSeats(string trainId);
        TrainSnapshot GetTrainSnapshot(string trainId);

        void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, Seats seats);
    }
}