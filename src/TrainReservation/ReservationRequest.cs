namespace KataTrainReservation
{
    public class ReservationRequest
    {
        public string TrainId { get; private set; }
        public int SeatCount { get; private set; }

        public ReservationRequest(string trainId, int seatCount)
        {
            this.TrainId = trainId;
            this.SeatCount = seatCount;
        }
    }
}