namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class Booking
    {
        public Booking(string eventId, string sessionID, byte propertyID, DateTime checkIn, DateTime checkOut, byte? noAdult, byte? noChildren,
                        byte? noInfant, string name, string email, string telephone, string postalCode, string country, long? total)
        {
            EventID = eventId;
            SessionID = sessionID;
            PropertyID = propertyID;
			CheckIn = checkIn;
			CheckOut = checkOut;
			NoAdult = noAdult;
            NoChildren = noChildren;
            NoInfant = noInfant;
            Name = name;
            Email = email;
            Telephone = telephone;
            PostalCode = postalCode;
            Country = country;
            Total = total;
        }

        public Booking(int id, string eventID, string sessionID, byte propertyID, DateTime checkIn, DateTime checkOut, byte? noAdult, byte? noChildren,
                        byte? noInfant, string name, string email, string telephone, string postalCode, string country, long? total)
        {
            ID = id;
            EventID = eventID;
            SessionID = sessionID;
            PropertyID = propertyID;
            CheckIn = checkIn;
            CheckOut = checkOut;
            NoAdult = noAdult;
            NoChildren = noChildren;
            NoInfant = noInfant;
            Name = name;
            Email = email;
            Telephone = telephone;
            PostalCode = postalCode;
            Country = country;
            Total = total;
        }

        public int ID { get; init; }
        public string EventID { get; init; }
        public string SessionID { get; init; }
        public byte PropertyID { get; init; }
        public DateTime CheckIn { get; init; }
        public DateTime CheckOut { get; init; }
        public byte? NoAdult { get; init; }
        public byte? NoChildren { get; init; }
        public byte? NoInfant { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string Telephone { get; init; }
        public string PostalCode { get; init; }
        public string Country { get; init; }
        public long? Total { get; init; }
    }
}
