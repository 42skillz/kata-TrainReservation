using NFluent;
using NUnit.Framework;
using TrainReservation.Domain.Core;
using TrainReservation.Tests.Helpers;

namespace TrainReservation.Tests
{
    [TestFixture]
    public class CoachTests
    {
        [Test]
        public void Should_say_it_has_enough_available_seats_when_below_70_percent_of_its_capacity()
        {
            var coach = new Coach("A-train", TrainProviderHelper.GetSeatsWithBookingReferencesFor1CoachesOf10SeatsAnd6ReservedSeats());

            var ask1Seat = 1;
            Check.That(coach.HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(ask1Seat)).IsTrue();

            var ask2Seats = 2;
            Check.That(coach.HasEnoughAvailableSeatsIfWeFollowTheIdealPolicy(ask2Seats)).IsFalse();
        }

        [Test]
        public void Should_be_a_value_type()
        {
            var firstInstance = new Coach("A-train", TrainProviderHelper.GetSeatsWithBookingReferencesFor1CoachesOf10SeatsAnd6ReservedSeats());
            var secondInstance = new Coach("A-train", TrainProviderHelper.GetSeatsWithBookingReferencesFor1CoachesOf10SeatsAnd6ReservedSeats());

            Check.That(firstInstance).IsEqualTo(secondInstance);
        }
    }
}