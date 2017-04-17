using TrainReservation.Domain;
using TrainReservation.Domain.Core;
using TrainReservation.Mocks;

namespace TrainReservation.Infra.Cli.Adapters
{
    internal class TrainDataProviderAdapter : IProvideTrainData
    {
        private readonly TrainDataServiceMock trainDataService;

        public TrainDataProviderAdapter()
        {
            trainDataService = new TrainDataServiceMock(TrainProviderHelper.GetTrainWith2CoachesAnd2IndividualSeatsAvailable("A-train"));
        }

        public TrainSnapshot GetTrainSnapshot(string trainId)
        {
            // the place where we should adapt the domain format into the 
            // json whatever needed by the external service to call (here, we'll call an
            // in-memory stub

            return trainDataService.GetTrainSnapshot(trainId);
        }

        public void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, Seats seats)
        {
            // the place where we should adapt the domain format into the 
            // json whatever needed by the external service to call (here, we'll call an
            // in-memory stub

            trainDataService.MarkSeatsAsReserved(trainId, bookingReference, seats);
        }
    }
}