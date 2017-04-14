using KataTrainReservation;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using TrainReservation.Domain;

namespace TrainReservation.Tests
{
    [TestFixture]
    public class TicketOfficeTests
    {
        [Test]
        public void Should_reserve_seats_when_unreserved_seats_are_available()
        {
            // setup mocks
            var expectedBookingId = "75bcd15";
            var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();
            bookingReferenceProvider.GetBookingReference().Returns(new BookingReference(expectedBookingId));

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetTrain(trainId).Returns(TrainProviderHelper.GetTrainWith1CoachAnd3SeatsAvailable(trainId));

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 3));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3));
        }

        [Test]
        public void Should_mark_seats_as_reserved_once_reserved()
        {
            // setup mocks
            var expectedBookingId = "75bcd15";
            var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();
            bookingReferenceProvider.GetBookingReference().Returns(new BookingReference(expectedBookingId));

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetTrain(trainId).Returns(TrainProviderHelper.GetTrainWith1Coach3SeatsIncluding1Available(trainId));

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 1));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 2));

            // Check that the train data mock has been called to persist this reservation
            trainDataProvider.Received().MarkSeatsAsReserved(trainId, reservation.BookingReference, reservation.Seats);
        }

        [Test]
        public void Should_not_reserve_more_than_70_percent_of_seats_for_overall_train()
        {
            // setup mocks
            var expectedBookingId = "75bcd15";
            var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();
            bookingReferenceProvider.GetBookingReference().Returns(new BookingReference(expectedBookingId));

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetTrain(trainId).Returns(TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable(trainId));

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 10));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3), new Seat("A", 4), new Seat("A", 5), new Seat("A", 6), new Seat("A", 7));
        }
    }
}
