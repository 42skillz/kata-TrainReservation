using System;
using System.Linq;
using TrainReservation.Domain;

namespace TrainReservation.Mocks
{
    public class BookingReferenceServiceMock : IProvideBookingReferences
    {
        private static readonly Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public BookingReference GetBookingReference()
        {
            return new BookingReference(RandomString(6));
        }
    }
}