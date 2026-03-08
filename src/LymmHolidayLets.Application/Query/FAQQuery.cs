using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.Model.FAQ.Entity;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class FAQQuery : IFAQQuery
    {
        private readonly IFAQRepository _faqRepository;

        public FAQQuery(IFAQRepository faqRepository)
        {
            _faqRepository = faqRepository;
        }

        public FAQ? GetById(int id)
        {
            return _faqRepository.GetById(id);
        }

        public IEnumerable<FAQ> GetAll()
        {
            return _faqRepository.GetAll();
        }
    }
}
