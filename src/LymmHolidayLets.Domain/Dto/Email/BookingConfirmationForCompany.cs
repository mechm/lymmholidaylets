namespace LymmHolidayLets.Domain.Dto.Email
{
    public sealed class BookingConfirmationForCompany
    {
        public BookingConfirmationForCompany(string propertyName, DateOnly checkIn, DateOnly checkOut, short? noAdult, short? noChildren,
                        short? noInfant, string name, string email, string telephone, string postalCode, string country, long? total)
        {
            PropertyName = propertyName;            
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

        public string PropertyName { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public short? NoAdult { get; init; }
        public short? NoChildren { get; init; }
        public short? NoInfant { get; init; }
        public string Name { get; init; }
        public string Email { get; init; }
        public string Telephone { get; init; }
        public string PostalCode { get; init; }
        public string Country { get; init; }
        public long? Total { get; init; }
    }
}
