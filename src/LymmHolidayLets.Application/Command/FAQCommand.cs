using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class FAQCommand(IFAQRepository faqRepository) : IFAQCommand
    {
        public void Create(FAQ faq)
        {
            faqRepository.Create(
                new Domain.Model.FAQ.Entity.FAQ(faq.PropertyID,faq.Question,faq.Answer,faq.Visible));
        }

        public void Update(FAQ faq)
        {
            faqRepository.Update(
               new Domain.Model.FAQ.Entity.FAQ(faq.ID, faq.PropertyID, faq.Question, faq.Answer, faq.Visible));
        }

        public void Delete(int id)
        {
            faqRepository.Delete(id);
        }
    }
}
