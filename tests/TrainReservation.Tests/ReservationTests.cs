using System.Collections.Generic;
using NFluent;
using NUnit.Framework;
using TrainReservation.Domain;

namespace TrainReservation.Tests
{
    [TestFixture]
    public class ReservationTests
    {
        [Test]
        public void Should_be_a_value_type()
        {
            var firstInstance = new Reservation("A-train", new BookingReference("ERklsks402"), new Seats(new List<Seat>() { new Seat("A", 24) }));
            var secondInstance = new Reservation("A-train", new BookingReference("ERklsks402"), new Seats(new List<Seat>() { new Seat("A", 24) }));

            Check.That(firstInstance).IsEqualTo(secondInstance);
        }
    }
}