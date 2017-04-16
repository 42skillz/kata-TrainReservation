namespace TrainReservation.Domain
{
    /// <summary>
    /// Main entry point to reserve train seats.
    /// </summary>
    public interface IReserveTrainSeats
    {
        void ReserveTrainSeats(string trainId, int numberOfSeats);
    }
}