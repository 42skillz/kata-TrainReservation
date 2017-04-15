namespace TrainReservation.Domain
{
    public interface IProvideTrainData
    {
        // List<SeatWithBookingReference> GetSeats(string trainId);
        Train GetTrain(string trainId);

        void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, Seats seats);
    }
}