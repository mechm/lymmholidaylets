using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class FAQCommand(
        IFAQRepository faqRepository,
        IPropertyCacheInvalidator cacheInvalidator) : IFAQCommand
    {
        public void Create(FAQ faq)
        {
            faqRepository.Create(
                new Domain.Model.FAQ.Entity.FAQ(faq.PropertyID, faq.Question, faq.Answer, faq.Visible));

            cacheInvalidator.Invalidate(faq.PropertyID);
        }

        public void Update(FAQ faq)
        {
            faqRepository.Update(
               new Domain.Model.FAQ.Entity.FAQ(faq.ID, faq.PropertyID, faq.Question, faq.Answer, faq.Visible));

            cacheInvalidator.Invalidate(faq.PropertyID);
        }

        public void Delete(int id)
        {
            var faq = faqRepository.GetById(id);
            faqRepository.Delete(id);

            if (faq is not null)
                cacheInvalidator.Invalidate(faq.PropertyID);
        }
    }
}
