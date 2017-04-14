using System.Collections.Generic;
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
            trainDataProvider.GetSeats(trainId).Returns(GetTrainWith1CoachAnd3SeatsAvailable());

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 3));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3));
        }

        private static List<SeatWithBookingReference> GetTrainWith1CoachAnd3SeatsAvailable()
        {
            return new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null) , new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null) };
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
            trainDataProvider.GetSeats(trainId).Returns(GetTrainWith1Coach3SeatsIncluding2Available());

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 1));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 2));

            // Check that the train data mock has been called to persist this reservation
            trainDataProvider.Received().MarkSeatsAsReserved(trainId, reservation.BookingReference, reservation.Seats);
        }

        private static List<SeatWithBookingReference> GetTrainWith1Coach3SeatsIncluding2Available()
        {
            return new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), new BookingReference("34Dsq")), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 3), new BookingReference("34Dsq")) };
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
            trainDataProvider.GetSeats(trainId).Returns(GetTrainWith1CoachAnd10SeatsAvailable());

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 10));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3), new Seat("A", 4), new Seat("A", 5), new Seat("A", 6), new Seat("A", 7));
        }

        public static List<SeatWithBookingReference> GetTrainWith1CoachAnd10SeatsAvailable()
        {
            return new List<SeatWithBookingReference>()
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
            };
        }
    }
}
