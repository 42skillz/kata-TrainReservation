using System.Collections.Generic;
using System.Linq;
using TrainReservation.Domain;
using TrainReservation.Domain.Core;

namespace TrainReservation.Mocks
{
    public class TrainDataServiceMock : IProvideTrainData
    {
        private readonly Dictionary<string, TrainSnapshot> trainSnapshots = new Dictionary<string, TrainSnapshot>();

        public TrainDataServiceMock(TrainSnapshot initialSnapshot)
        {
            trainSnapshots[initialSnapshot.TrainId] = initialSnapshot;
        }

        public TrainSnapshot GetTrainSnapshot(string trainId)
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

            var updatedSnapshot = new TrainSnapshot(trainId, newSeatsConfiguration);
            trainSnapshots[trainId] = updatedSnapshot;
        }
    }
}