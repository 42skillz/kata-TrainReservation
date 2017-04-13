using System.Collections.Generic;
using KataTrainReservation;

namespace TrainReservation.Domain
{
    public class TicketOffice
    {
        private readonly IProvideBookingReferences bookingReferenceProvider;
        private readonly IProvideTrainData trainDataProvider;

        public TicketOffice(IProvideBookingReferences bookingReferenceProvider, IProvideTrainData trainDataProvider)
        {
            this.bookingReferenceProvider = bookingReferenceProvider;
            this.trainDataProvider = trainDataProvider;
        }

        public Reservation MakeReservation(ReservationRequest request)
        {
            var reservedSeats = new List<Seat>();

            var seats = trainDataProvider.GetSeats(request.TrainId);
            foreach (var seatWithBookingReference in seats)
            {
                if (seatWithBookingReference.IsAvailable())
                {
                    reservedSeats.Add(seatWithBookingReference.Seat);
                }
            }

            if (reservedSeats.Count > 0)
            {
                var bookingReference = bookingReferenceProvider.GetBookingReference();

                trainDataProvider.MarkSeatsAsReserved(request.TrainId, bookingReference, reservedSeats);

                return new Reservation(request.TrainId, bookingReference, reservedSeats);
            }

            return new Reservation(request.TrainId, string.Empty, reservedSeats);
        }
    }
}