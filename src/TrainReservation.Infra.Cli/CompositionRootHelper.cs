using TrainReservation.Domain;
using TrainReservation.Domain.Services;
using TrainReservation.Infra.Cli.Adapters;

namespace TrainReservation.Infra.Cli
{
    public static class CompositionRootHelper
    {
        /// <summary>
        /// Acts like a composition root for the Hexagonal Architecture.
        /// </summary>
        public static IReserveTrainSeats ComposeTheHexagon(IProvideBookingReferences bookingReferenceProvider, IProvideTrainData trainDataProvider)
        {
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);

            var reservationAdapter = new CliReservationAdapter(ticketOffice);

            return reservationAdapter;
        }
    }
}