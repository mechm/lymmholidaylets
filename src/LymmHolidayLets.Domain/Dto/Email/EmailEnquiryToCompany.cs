namespace LymmHolidayLets.Domain.Dto.Email
{
    public sealed class EmailEnquiryToCompany
    {
        public required string Name { get; set; }
        public string? Company { get; set; }
        public string? EmailAddress { get; set; }
        public string? TelephoneNo { get; set; }
        public string? Subject { get; set; }
        public required string Message { get; set; }
        public DateTime Created { get; set; }
    }
}
