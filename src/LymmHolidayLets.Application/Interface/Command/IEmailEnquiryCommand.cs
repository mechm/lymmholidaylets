
using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface IEmailEnquiryCommand
    {
        void Create(EmailEnquiry emailEnquiry);
        void Update(EmailEnquiry emailEnquiry);
        void Delete(int id);
    }
}
