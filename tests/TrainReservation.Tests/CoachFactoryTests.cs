using NFluent;
using NUnit.Framework;
using TrainReservation.Domain;
using TrainReservation.Tests.Helpers;

namespace TrainReservation.Tests
{
    [TestFixture]
    public class CoachFactoryTests
    {
        [Test]
        public void Should_instantiate_coaches_from_seats_with_booking_references()
        {
            var coaches = CoachFactory.InstantiateCoaches("the A-train", TrainProviderHelper.GetSeatsWithBookingReferencesFor2CoachesOf10Seats());
            Check.That(coaches).HasSize(2);
            Check.That(coaches["A"]).IsNotNull();
            Check.That(coaches["A"].Seats).HasSize(10);
            Check.That(coaches["B"]).IsNotNull();
            Check.That(coaches["B"].Seats).HasSize(10);
        }
    }
}