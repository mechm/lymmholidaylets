using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class FAQCommand : IFAQCommand
    {
        private readonly IDapperFAQRepository _faqRepository;

        public FAQCommand(IDapperFAQRepository faqRepository)
        {
            _faqRepository = faqRepository;
        }

        public void Create(FAQ faq)
        {
            _faqRepository.Create(
                new Domain.Model.FAQ.Entity.FAQ(faq.PropertyID,faq.Question,faq.Answer,faq.Visible));
        }

        public void Update(FAQ faq)
        {
            _faqRepository.Update(
               new Domain.Model.FAQ.Entity.FAQ(faq.ID, faq.PropertyID, faq.Question, faq.Answer, faq.Visible));
        }

        public void Delete(int id)
        {
            _faqRepository.Delete(id);
        }
    }
}