using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class EmailEnquiryCommand(IEmailEnquiryRepository emailEnquiryRepository) : IEmailEnquiryCommand
    {
        public void Create(EmailEnquiry emailEnquiry)
        {
            emailEnquiryRepository.Create(
               new Domain.Model.EmailEnquiry.Entity.EmailEnquiry(emailEnquiry.Name,
               emailEnquiry.Company, emailEnquiry.EmailAddress, emailEnquiry.TelephoneNo,
               emailEnquiry.Subject, emailEnquiry.Message));
        }

        public void Update(EmailEnquiry emailEnquiry)
        {
            var emailEnquiryToUpdate = emailEnquiryRepository.GetById(emailEnquiry.EmailEnquiryId);
            if (emailEnquiryToUpdate != null)
            {
                emailEnquiryRepository.Update(
                   new Domain.Model.EmailEnquiry.Entity.EmailEnquiry(emailEnquiry.EmailEnquiryId, emailEnquiry.Name,
                  emailEnquiry.Company, emailEnquiry.EmailAddress, emailEnquiry.TelephoneNo,
                  emailEnquiry.Subject, emailEnquiry.Message, emailEnquiryToUpdate.DateTimeOfEnquiry));
            }
        }

        public void Delete(int id)
        {
            emailEnquiryRepository.Delete(id);
        }
    }
}
