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
            return bookingReferenceService.GetBookingReference();
        }
    }
}