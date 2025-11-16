using LymmHolidayLets.Domain.Model.FAQ.Entity;

namespace LymmHolidayLets.Domain.Repository
{  
    public interface IDapperFAQRepository : IDapperRepository<FAQ>
    {
        FAQ? GetById(int id);
        IEnumerable<FAQ> GetAll();
        void Create(FAQ faq);
        void Update(FAQ faq);
        void Delete(int id);
    }
}
