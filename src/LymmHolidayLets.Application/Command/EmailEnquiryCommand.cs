using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class EmailEnquiryCommand : IEmailEnquiryCommand
    {
        private readonly IDapperEmailEnquiryRepository _emailEnquiryRepository;

        public EmailEnquiryCommand(IDapperEmailEnquiryRepository emailEnquiryRepository)
        {
            _emailEnquiryRepository = emailEnquiryRepository;
        }

        public void Create(EmailEnquiry emailEnquiry)
        {
            _emailEnquiryRepository.Create(
               new Domain.Model.EmailEnquiry.Entity.EmailEnquiry(emailEnquiry.Name,
               emailEnquiry.Company, emailEnquiry.EmailAddress, emailEnquiry.TelephoneNo,
               emailEnquiry.Subject, emailEnquiry.Message));
        }

        public void Update(EmailEnquiry emailEnquiry)
        {
            var emailEnquiryToUpdate = _emailEnquiryRepository.GetById(emailEnquiry.EmailEnquiryId);
            if (emailEnquiryToUpdate != null)
            {
                _emailEnquiryRepository.Update(
                   new Domain.Model.EmailEnquiry.Entity.EmailEnquiry(emailEnquiry.EmailEnquiryId, emailEnquiry.Name,
                  emailEnquiry.Company, emailEnquiry.EmailAddress, emailEnquiry.TelephoneNo,
                  emailEnquiry.Subject, emailEnquiry.Message, emailEnquiryToUpdate.DateTimeOfEnquiry));
            }
        }

        public void Delete(int id)
        {
            _emailEnquiryRepository.Delete(id);
        }
    }
}
