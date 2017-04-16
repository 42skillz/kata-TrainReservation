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

        public TrainSnapshotForReservation GetTrainSnapshot(string trainId)
        {
            return trainDataService.GetTrainSnapshot(trainId);
        }

        public void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, Seats seats)
        {
            trainDataService.MarkSeatsAsReserved(trainId, bookingReference, seats);
        }
    }
}