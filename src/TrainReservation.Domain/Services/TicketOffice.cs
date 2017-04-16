namespace TrainReservation.Domain.Services
{
    /// <summary>
    /// Service to reserve train tickets.
    /// </summary>
    public class TicketOffice
    {
        private readonly IProvideBookingReferences bookingReferenceProvider;
        private readonly IProvideTrainData trainDataProvider;

        public TicketOffice(IProvideBookingReferences bookingReferenceProvider, IProvideTrainData trainDataProvider)
        {
            this.bookingReferenceProvider = bookingReferenceProvider;
            this.trainDataProvider = trainDataProvider;
        }

        public Reservation MakeReservation(ReservationRequest request)
        {
            var trainSnapshot = this.trainDataProvider.GetTrainSnapshot(request.TrainId);
            var option = trainSnapshot.FindReservationOption(request.SeatCount);

            if (option.IsFullfiled)
            {
                // Call the various services to validate the transaction
                var bookingReference = this.bookingReferenceProvider.GetBookingReference();

                this.trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, option.ReservedSeats);
                return new Reservation(request.TrainId, bookingReference, option.ReservedSeats);
            }
            else
            {
                return new Reservation(request.TrainId);
            }
        }
    }
}