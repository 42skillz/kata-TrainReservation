using System;
using System.Text;
using TrainReservation.Domain;
using TrainReservation.Domain.Services;

namespace TrainReservation.Infra.Cli.Adapters
{
    public class CliReservationAdapter : IReserveTrainSeats
    {
        private readonly TicketOffice ticketOffice;

        public CliReservationAdapter(TicketOffice ticketOffice)
        {
            this.ticketOffice = ticketOffice;
        }

        public void ReserveTrainSeats(string trainId, int numberOfSeats)
        {
            // Call the domain logic
            var reservation = this.ticketOffice.MakeReservation(new ReservationRequest(trainId, numberOfSeats));

            // Adapt
            string json = AdaptInJSON(reservation);

            // Publish
            Console.WriteLine(json);
        }

        public static string AdaptInJSON(Reservation reservation)
        {
            return $"{{\"train_id\": \"{reservation.TrainId}\", \"booking_reference\": \"{reservation.BookingReference}\", \"seats\": [{SerializeSeats(reservation.Seats)}]}}";
        }

        private static string SerializeSeats(Seats seats)
        {
            var result = new StringBuilder();

            var firstElement = true;
            foreach (var seat in seats)
            {
                if (!firstElement)
                {
                    result.Append(", ");
                }
                
                result.Append($"\"{seat.SeatNumber}{seat.Coach}\"");

                firstElement = false;
            }

            return result.ToString();
        }
    }
}