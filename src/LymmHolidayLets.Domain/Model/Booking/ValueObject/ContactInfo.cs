namespace LymmHolidayLets.Domain.Model.Booking.ValueObject
{
    public sealed record ContactInfo
    {
        public string Name { get; }
        public string Email { get; }
        public string Telephone { get; }

        public ContactInfo(string name, string email, string telephone)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required.");
            // Add email/phone validation if needed
            Name = name;
            Email = email;
            Telephone = telephone;
        }
    }
}
