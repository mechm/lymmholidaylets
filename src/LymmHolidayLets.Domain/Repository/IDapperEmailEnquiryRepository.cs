using LymmHolidayLets.Domain.Model.EmailEnquiry.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IDapperEmailEnquiryRepository : IDapperRepository<EmailEnquiry>
    {
        EmailEnquiry? GetById(int id);
        IEnumerable<EmailEnquiry> GetAll();
        void Create(EmailEnquiry emailEnquiry);
        void Update(EmailEnquiry emailEnquiry);
        void Delete(int id);
    }
}
