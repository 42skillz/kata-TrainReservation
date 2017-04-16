using System;
using TrainReservation.Domain;
using TrainReservation.Infra.Cli.Adapters;

namespace TrainReservation.Infra.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Welcome to the TrainReservation service.");
                Console.WriteLine("usage: [Train id] [number of seats]");
                Console.WriteLine("");
            }

            var trainId = args[0];
            var numberOfSeats = int.Parse(args[1]);

            // instantiate adapters
            IProvideBookingReferences bookingReferenceAdapter = new BookingReferenceProviderAdapter();
            IProvideTrainData trainDataProviderAdapter = new TrainDataProviderAdapter();

            // instantiate the hexagon and retrieve the adapter to interact with
            var reservationAdapter = CompositionRootHelper.ComposeTheHexagon(bookingReferenceAdapter, trainDataProviderAdapter);

            // ask the adapter to reserve...
            reservationAdapter.ReserveTrainSeats(trainId, numberOfSeats);

            Console.WriteLine("Type enter to exit");
            Console.ReadLine();
        }
    }
}