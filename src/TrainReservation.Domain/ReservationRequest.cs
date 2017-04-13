namespace KataTrainReservation
{
    public class ReservationRequest
    {
        public string TrainId { get; }
        public int SeatCount { get; }

        public ReservationRequest(string trainId, int seatCount)
        {
            TrainId = trainId;
            SeatCount = seatCount;
        }
    }
}