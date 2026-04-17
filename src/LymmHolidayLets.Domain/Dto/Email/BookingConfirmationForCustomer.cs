namespace LymmHolidayLets.Domain.Dto.Email
{
    public sealed class BookingConfirmationForCustomer
    {
        public BookingConfirmationForCustomer()
        {
        }

        public BookingConfirmationForCustomer(string propertyName, DateOnly checkIn, DateOnly checkOut, short? noAdult, short? noChildren,
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

        public string PropertyName { get; init; } = string.Empty;
        public string? BookingReference { get; init; }
        public string? PropertyUrl { get; init; }
        public byte? Bedroom { get; init; }
        public double? Bathroom { get; init; }
        public DateOnly CheckIn { get; init; }
        public DateOnly CheckOut { get; init; }
        public short? NoAdult { get; init; }
        public short? NoChildren { get; init; }
        public short? NoInfant { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Telephone { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public long? Total { get; init; }
        public TimeOnly CheckInTimeAfter { get; init; }
        public TimeOnly CheckOutTimeBefore { get; init; }
        public string FullAddress { get; init; } = string.Empty;
        public string? DirectionsUrl { get; init; }
        public string? ArrivalInstructions { get; init; }
        public string? HeroImageUrl { get; init; }
        public string? HeroImageAltText { get; init; }
        public IReadOnlyList<string> HouseRules { get; init; } = [];
        public IReadOnlyList<string> SafetyItems { get; init; } = [];
        public string CancellationPolicyText { get; init; } = string.Empty;
        public IReadOnlyList<BookingConfirmationPaymentLine> PaymentLines { get; init; } = [];
    }

    public sealed record BookingConfirmationPaymentLine(string Label, decimal Amount);
}
