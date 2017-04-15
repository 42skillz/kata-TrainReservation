using System.Collections.Generic;
using System.Linq;
using TrainReservation.Domain;

namespace TrainReservation.Tests.Helpers
{
    internal class TrainDataProviderMock : IProvideTrainData
    {
        private readonly Dictionary<string, TrainSnapshotForReservation> trainSnapshots = new Dictionary<string, TrainSnapshotForReservation>();

        public TrainDataProviderMock(TrainSnapshotForReservation initialSnapshot)
        {
            trainSnapshots[initialSnapshot.TrainId] = initialSnapshot;
        }

        public TrainSnapshotForReservation GetTrainSnapshot(string trainId)
        {
            return trainSnapshots[trainId];
        }

        public void MarkSeatsAsReserved(string trainId, BookingReference bookingReference, Seats seats)
        {
            var formerSnapshot = trainSnapshots[trainId];
            var newSeatsConfiguration = new List<SeatWithBookingReference>(formerSnapshot.OverallTrainCapacity);
            foreach (var seatWithBookingReference in formerSnapshot.SeatsWithBookingReferences)
            {
                if (seats.Contains(seatWithBookingReference.Seat))
                {
                    // must keep the new reservation id for this seat 
                    var newBookingReference = new SeatWithBookingReference(seatWithBookingReference.Seat, bookingReference);
                    newSeatsConfiguration.Add(newBookingReference);
                }
                else
                {
                    // keep the former value
                    newSeatsConfiguration.Add(seatWithBookingReference);
                }
            }

            var updatedSnapshot = new TrainSnapshotForReservation(trainId, newSeatsConfiguration);
            trainSnapshots[trainId] = updatedSnapshot;
        }
    }
}