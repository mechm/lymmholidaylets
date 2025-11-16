using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Model.EmailEnquiry.Entity
{
    public sealed class EmailEnquiry : IAggregateRoot
    {
        // read and update
        public EmailEnquiry(short emailEnquiryId, string name, string? company, string? emailAddress,
            string? telephoneNo, string? subject, string message, DateTime dateTimeOfEnquiry)
        {
            EmailEnquiryId = emailEnquiryId;
            Name = name;
            Company = company;
            EmailAddress = emailAddress;
            TelephoneNo = telephoneNo;
            Subject = subject;
            Message = message;
            DateTimeOfEnquiry = dateTimeOfEnquiry;
        }


        // create
        public EmailEnquiry(string name, string? company, string? emailAddress,
            string? telephoneNo, string? subject, string message)
        {
            Name = name;
            Company = company;
            EmailAddress = emailAddress;
            TelephoneNo = telephoneNo;
            Subject = subject;
            Message = message;
            DateTimeOfEnquiry = DateTime.UtcNow;
        }

        public short EmailEnquiryId { get; set; }
        public string Name { get; set; }
        public string? Company { get; set; }
        public string? EmailAddress { get; set; }
        public string? TelephoneNo { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; }
        public DateTime DateTimeOfEnquiry { get; set; }
    }
}
