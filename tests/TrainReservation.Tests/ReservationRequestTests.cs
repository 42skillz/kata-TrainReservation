using NFluent;
using NUnit.Framework;
using TrainReservation.Domain;

namespace TrainReservation.Tests
{
    [TestFixture]
    public class ReservationRequestTests
    {
        [Test]
        public void Should_be_a_value_type()
        {
            var firstInstance = new ReservationRequest("A-train", 3);
            var secondInstance = new ReservationRequest("A-train", 3);

            Check.That(firstInstance).IsEqualTo(secondInstance);
        }
    }
}