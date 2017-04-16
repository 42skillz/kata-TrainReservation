using System.Collections.Generic;
using NFluent;
using NUnit.Framework;
using TrainReservation.Domain;
using TrainReservation.Infra.Cli.Adapters;

namespace TrainReservation.Tests.Infra
{
    [TestFixture]
    public class CliReservationAdapterTests
    {
        [Test]
        public void Should_serialize_in_JSON_as_expected()
        {
            var reservation = new Reservation("A-train", new BookingReference("RefDe0uf"), new Seats(new List<Seat>(){ new Seat("A", 4) , new Seat("A", 5)}));
            var json = CliReservationAdapter.AdaptInJSON(reservation);

            Check.That(json).IsEqualTo($"{{\"train_id\": \"A-train\", \"booking_reference\": \"RefDe0uf\", \"seats\": [\"4A\", \"5A\"]}}");
        }
    }
}
