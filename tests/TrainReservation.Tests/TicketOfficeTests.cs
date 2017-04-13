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
            bookingReferenceProvider.GetBookingReference().Returns(expectedBookingId);

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetSeats(trainId).Returns(new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null) , new SeatWithBookingReference(new Seat("A", 3), BookingReference.Null) });

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 3));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingId).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3));
        }

        [Test]
        public void Should_mark_seats_as_reserved_once_reserved()
        {
            // setup mocks
            var expectedBookingId = "75bcd15";
            var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();
            bookingReferenceProvider.GetBookingReference().Returns(expectedBookingId);

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetSeats(trainId).Returns(new List<SeatWithBookingReference>() { new SeatWithBookingReference(new Seat("A", 1), new BookingReference("34Dsq")), new SeatWithBookingReference(new Seat("A", 2), BookingReference.Null), new SeatWithBookingReference(new Seat("A", 3), new BookingReference("34Dsq")) });

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 1));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingId).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 2));

            trainDataProvider.Received().MarkSeatsAsReserved(trainId, new BookingReference(reservation.BookingId), new List<Seat>(){new Seat("A", 2)});
        }
    }
}
