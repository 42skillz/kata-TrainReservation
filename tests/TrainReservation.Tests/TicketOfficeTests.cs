using KataTrainReservation;
using NFluent;
using NSubstitute;
using NUnit.Framework;

namespace TrainReservation.Tests
{
    [TestFixture]
    public class TicketOfficeTests
    {
        [Test]
        public void Should_reserve_seats_when_unreserved_seats_are_available()
        {
            var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();
            string expectedBookingId = "75bcd15";
            bookingReferenceProvider.GetBookingReference().Returns(expectedBookingId);

            var trainDataProvider = new TrainDataProvider();

            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var trainId = "express_2000";

            var reservationRequest = new ReservationRequest(trainId, 3);
            var reservation = ticketOffice.Reserve(reservationRequest);

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingId).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3));
        }
    }

    public interface IProvideTrainData
    {
    }

    public class TrainDataProvider : IProvideTrainData
    {
    }

    public interface IProvideBookingReferences
    {
        string GetBookingReference();
    }

    public class BookingReferenceProvider : IProvideBookingReferences
    {
        public string GetBookingReference()
        {
            throw new System.NotImplementedException();
        }
    }

    public class TicketOffice
    {
        private IProvideBookingReferences bookingReferenceProvider;
        private IProvideTrainData trainDataProvider;

        public TicketOffice(IProvideBookingReferences bookingReferenceProvider, IProvideTrainData trainDataProvider)
        {
            this.bookingReferenceProvider = bookingReferenceProvider;
            this.trainDataProvider = trainDataProvider;
        }

        public Reservation Reserve(ReservationRequest reservationRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}
