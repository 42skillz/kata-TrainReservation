using System.Collections.Generic;
using TrainReservation.Domain;

namespace TrainReservation.Tests
{
    internal static class TrainProviderHelper
    {
        public static TrainSnapshotForReservation GetTrainWith1CoachAnd3SeatsAvailable(string trainId)
        {
            var train = new TrainSnapshotForReservation(trainId, new List<SeatWithBookingReference>()
            {
                new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null)
            });

            return train;
        }

        public static TrainSnapshotForReservation GetTrainWith1Coach3SeatsIncluding1Available(string trainId)
        {
            var train = new TrainSnapshotForReservation(trainId, new List<SeatWithBookingReference>()
            {
                new SeatWithBookingReference(new Seat("A", 1), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 3), new BookingReference("34Dsq"))
            });

            return train;
        }

        public static TrainSnapshotForReservation GetTrainWith1CoachAnd10SeatsAvailable(string trainId)
        {
            var train = new TrainSnapshotForReservation(trainId, new List<SeatWithBookingReference>()
            {
                new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 4), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 5), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 6), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 7), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 8), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 9), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 10), BookingReference.Null)
            });

            return train;
        }

        public static TrainSnapshotForReservation GetTrainWith2CoachesAnd2IndividualSeatsAvailable(string trainId)
        {
            var train = new TrainSnapshotForReservation(trainId, new List<SeatWithBookingReference>()
            {
                new SeatWithBookingReference(new Seat("A", 1), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("A", 2), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("A", 3), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("A", 4), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("A", 5), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("A", 6), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("A", 7), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 8), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 9), BookingReference.Null),
                new SeatWithBookingReference(new Seat("A", 10), BookingReference.Null),

                new SeatWithBookingReference(new Seat("B", 1), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("B", 2), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("B", 3), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("B", 4), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("B", 5), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("B", 6), new BookingReference("34Dsq")),
                new SeatWithBookingReference(new Seat("B", 7), BookingReference.Null),
                new SeatWithBookingReference(new Seat("B", 8), BookingReference.Null),
                new SeatWithBookingReference(new Seat("B", 9), BookingReference.Null),
                new SeatWithBookingReference(new Seat("B", 10), BookingReference.Null),
            });

            return train;
        }
    }
}