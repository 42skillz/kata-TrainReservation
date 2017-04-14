using System.Collections.Generic;
using KataTrainReservation;
using TrainReservation.Domain;

internal static class TrainProviderHelper
{
    public static Train GetTrainWith1CoachAnd3SeatsAvailable(string trainId)
    {
        var train = new Train(trainId, new List<SeatWithBookingReference>()
        {
            new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null),
            new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null),
            new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null)
        });

        return train;
    }

    public static Train GetTrainWith1Coach3SeatsIncluding1Available(string trainId)
    {
        var train = new Train(trainId, new List<SeatWithBookingReference>()
        { 
            new SeatWithBookingReference(new Seat("A", 1), new BookingReference("34Dsq")),
            new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null),
            new SeatWithBookingReference(new Seat("A", 3), new BookingReference("34Dsq"))
        });

        return train;
    }

    public static Train GetTrainWith1CoachAnd10SeatsAvailable(string trainId)
    {
        var train = new Train(trainId, new List<SeatWithBookingReference>()
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
}