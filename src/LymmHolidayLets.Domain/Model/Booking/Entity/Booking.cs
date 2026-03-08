using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Model.Booking.ValueObject;

namespace LymmHolidayLets.Domain.Model.Booking.Entity
{
    public sealed class Booking : IAggregateRoot
    {
        public int ID { get; private set; }
        public string EventID { get; private set; }
        public string SessionID { get; private set; }
        public byte PropertyID { get; private set; }
        public StayPeriod Period { get; private set; }
        public byte? NoAdult { get; private set; }
        public byte? NoChildren { get; private set; }
        public byte? NoInfant { get; private set; }
        public byte? NoOfGuests { get; private set; }
        public ContactInfo Contact { get; private set; }
        public string PostalCode { get; private set; }
        public string Country { get; private set; }
        public long? Total { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Updated { get; private set; }

        // Factory for new bookings
        public static Booking CreateNew(string eventID, string sessionID, byte propertyID, StayPeriod period, ContactInfo contact,
                                       byte? noAdult, byte? noChildren, byte? noInfant, string postalCode, string country, long? total)
        {
            return new Booking(eventID, sessionID, propertyID, period, contact, noAdult, noChildren, noInfant, postalCode, country, total);
        }

        private Booking(string eventID, string sessionID, byte propertyID, StayPeriod period, ContactInfo contact,
                        byte? noAdult, byte? noChildren, byte? noInfant, string postalCode, string country, long? total)
        {
            EventID = eventID;
            SessionID = sessionID;
            PropertyID = propertyID;
            Period = period;
            Contact = contact;
            NoAdult = noAdult;
            NoChildren = noChildren;
            NoInfant = noInfant;
            PostalCode = postalCode;
            Country = country;
            Total = total;
            Created = DateTime.UtcNow;
        }

        // Keep existing constructors for Dapper/Old code
        public Booking(string eventID, string sessionID, byte propertyID, DateTime checkIn, DateTime checkOut, byte? noAdult, byte? noChildren,
                        byte? noInfant, string name, string email, string telephone, string postalCode, string country, long? total)
        {
            EventID = eventID;
            SessionID = sessionID;
            PropertyID = propertyID;
            Period = new StayPeriod(checkIn, checkOut);
            Contact = new ContactInfo(name, email, telephone);
            NoAdult = noAdult;
            NoChildren = noChildren;
            NoInfant = noInfant;
            PostalCode = postalCode;
            Country = country;
            Total = total;
            Created = DateTime.UtcNow;
        }

        public Booking(int id, string eventID, string sessionID, byte propertyID, DateTime checkIn, DateTime checkOut, byte? noAdult, byte? noChildren,
                        byte? noInfant, string name, string email, string telephone, string postalCode, string country, long? total)
        {
            ID = id;
            EventID = eventID;
            SessionID = sessionID;
            PropertyID = propertyID;
            Period = new StayPeriod(checkIn, checkOut);
            Contact = new ContactInfo(name, email, telephone);
            NoAdult = noAdult;
            NoChildren = noChildren;
            NoInfant = noInfant;
            PostalCode = postalCode;
            Country = country;
            Total = total;
            Updated = DateTime.UtcNow;
        }

        public Booking(int id, string eventID, string sessionID, byte propertyID, DateTime checkIn, DateTime checkOut, byte? noAdult, byte? noChildren,
                        byte? noInfant, byte? noOfGuests, string name, string email, string telephone, string postalCode, string country, long? total, DateTime created, DateTime? updated)
        {
            ID = id;
            EventID = eventID;
            SessionID = sessionID;
            PropertyID = propertyID;
            Period = new StayPeriod(checkIn, checkOut);
            Contact = new ContactInfo(name, email, telephone);
            NoAdult = noAdult;
            NoChildren = noChildren;
            NoInfant = noInfant;
            NoOfGuests = noOfGuests;
            PostalCode = postalCode;
            Country = country;
            Total = total;
            Created = created;
            Updated = updated;
        }

        // Backwards compatibility properties (can be deprecated later)
        public DateTime CheckIn => Period.CheckIn;
        public DateTime CheckOut => Period.CheckOut;
        public string Name => Contact.Name;
        public string Email => Contact.Email;
        public string Telephone => Contact.Telephone;
    }
}
