using TrainReservation.Domain;
using TrainReservation.Mocks;

namespace TrainReservation.Infra.Cli.Adapters
{
    internal class BookingReferenceProviderAdapter : IProvideBookingReferences
    {
        private readonly BookingReferenceServiceMock bookingReferenceService;

        public BookingReferenceProviderAdapter()
        {
            bookingReferenceService = new BookingReferenceServiceMock();
        }

        public BookingReference GetBookingReference()
        {
            // the place where we should adapt the domain format into the 
            // json whatever needed by the external service to call (here, we'll call an
            // in-memory stub

            return bookingReferenceService.GetBookingReference();
        }
    }
}