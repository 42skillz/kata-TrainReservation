﻿using System.Collections.Generic;
using Value;

namespace TrainReservation.Domain
{
    public class SeatWithBookingReference : ValueType<SeatWithBookingReference>
    {
        public Seat Seat { get; }
        public BookingReference BookingReference { get; }

        public SeatWithBookingReference(Seat seat, BookingReference bookingReference)
        {
            Seat = seat;
            BookingReference = bookingReference;
        }

        public bool IsAvailable => BookingReference.Equals(BookingReference.Null);

        protected override IEnumerable<object> GetAllAttributesToBeUsedForEquality()
        {
            return new object[] {Seat, BookingReference};
        }

        public override string ToString()
        {
            return $"[{Seat}-{BookingReference}]";
        }
    }
}