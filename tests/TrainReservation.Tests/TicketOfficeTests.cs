using System.Collections.Generic;
using System.Linq;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using TrainReservation.Domain;
using TrainReservation.Tests.Helpers;

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
            trainDataProvider.GetTrainSnapshot(trainId).Returns(TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable(trainId));

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
            var bookingReferenceProvider = InstantiateBookingReferenceProviderMock(expectedBookingId);

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetTrainSnapshot(trainId).Returns(TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable(trainId));

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 1));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats).ContainsExactly(new Seat("A", 1));

            // Check that the train data mock has been called to persist this reservation
            trainDataProvider.Received().MarkSeatsAsReserved(trainId, reservation.BookingReference, reservation.Seats);
        }

        private static IProvideBookingReferences InstantiateBookingReferenceProviderMock(string[] expectedBookingIds)
        {
            var expectedBookingReferences = new List<BookingReference>();
            foreach (var bookingId in expectedBookingIds)
            {
                expectedBookingReferences.Add(new BookingReference(bookingId));
            }

            var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();

            bookingReferenceProvider.GetBookingReference()
                .Returns(expectedBookingReferences.First(), expectedBookingReferences.GetRange(1, expectedBookingReferences.Count - 1).ToArray());
            return bookingReferenceProvider;
        }

        private static IProvideBookingReferences InstantiateBookingReferenceProviderMock(string expectedBookingId)
        {
            var bookingReferenceProvider = Substitute.For<IProvideBookingReferences>();
            bookingReferenceProvider.GetBookingReference().Returns(new BookingReference(expectedBookingId));
            return bookingReferenceProvider;
        }

        [Test]
        public void Should_be_able_to_reserve_70_percent_of_overall_train_seats_capacity()
        {
            // setup mocks
            var expectedBookingId = "75bcd15";
            var bookingReferenceProvider = InstantiateBookingReferenceProviderMock(expectedBookingId);

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetTrainSnapshot(trainId).Returns(TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable(trainId));

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 7));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEqualTo(expectedBookingId);
            Check.That(reservation.Seats)
                .ContainsExactly(new Seat("A", 1), new Seat("A", 2), new Seat("A", 3), new Seat("A", 4), new Seat("A", 5), new Seat("A", 6), new Seat("A", 7));
        }

        [Test]
        public void Should_fail_to_reserve_when_asking_more_seats_than_the_70_percent_limit()
        {
            // setup mocks
            var expectedBookingId = "75bcd15";
            var bookingReferenceProvider = InstantiateBookingReferenceProviderMock(expectedBookingId);

            var trainId = "express_2000";
            var trainDataProvider = Substitute.For<IProvideTrainData>();
            trainDataProvider.GetTrainSnapshot(trainId).Returns(TrainProviderHelper.GetTrainWith1CoachAnd10SeatsAvailable(trainId));

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var reservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 9));

            Check.That(reservation.TrainId).IsEqualTo(trainId);
            Check.That(reservation.BookingReference.Value).IsEmpty();
            Check.That(reservation.Seats).IsEmpty();
        }

        [Test]
        public void Should_ideally_left_70_percent_of_every_coach_available()
        {
            // setup mocks
            var firstBookingId = "75bcd15";
            var secondBookingId = "9904fgG6";

            var bookingReferenceProvider = InstantiateBookingReferenceProviderMock(new[] {firstBookingId, secondBookingId});

            var trainId = "express_2000";
            var trainDataProvider = new TrainDataProviderMock(TrainProviderHelper.GetTrainWith2CoachesAnd2IndividualSeatsAvailable(trainId));

            // act
            // First reservation
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var firstReservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 1));

            // Must be located in the remaining seat of 1st coach
            Check.That(firstReservation.TrainId).IsEqualTo(trainId);
            Check.That(firstReservation.BookingReference.Value).IsEqualTo(firstBookingId);
            Check.That(firstReservation.Seats).ContainsExactly(new Seat("A", 7));

            // Second reservation
            var secondReservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 1));

            // Must be located in the remaining seat of 1st coach
            Check.That(secondReservation.TrainId).IsEqualTo(trainId);
            Check.That(secondReservation.BookingReference.Value).IsEqualTo(secondBookingId);
            Check.That(secondReservation.Seats).ContainsExactly(new Seat("B", 7));
        }

        [Test]
        public void Should_break_the_70_percent_of_every_coach_rule_when_no_alternative()
        {
            // setup mocks
            var firstBookingId = "75bcd15";
            var secondBookingId = "9904fgG6";

            var bookingReferenceProvider = InstantiateBookingReferenceProviderMock(new[] {firstBookingId, secondBookingId});

            var trainId = "express_2000";
            var trainDataProvider = new TrainDataProviderMock(TrainProviderHelper.GetTrainWith2CoachesAnd2IndividualSeatsAvailable(trainId));

            // act
            var ticketOffice = new TicketOffice(bookingReferenceProvider, trainDataProvider);
            var firstReservation = ticketOffice.MakeReservation(new ReservationRequest(trainId, 2));

            // Must be located in the remaining seat of 1st coach
            Check.That(firstReservation.TrainId).IsEqualTo(trainId);
            Check.That(firstReservation.BookingReference.Value).IsEqualTo(firstBookingId);
            Check.That(firstReservation.Seats).ContainsExactly(new Seat("A", 7), new Seat("A", 8));
        }
    }
}