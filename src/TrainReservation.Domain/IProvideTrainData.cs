namespace TrainReservation.Domain
{
    public interface IProvideTrainData
    {
        // List<SeatWithBookingReference> GetSeats(string trainId);
        TrainSnapshotForReservation GetTrainSnapshot(string trainId);

        void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, Seats seats);
    }
}