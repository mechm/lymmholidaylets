using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.ReadModel.Page;

namespace LymmHolidayLets.Domain.DataAdapter
{
    public interface IDapperTemplateDataAdapter : IDapperSqlQueryBase
    {
        IEnumerable<Template> GetAllTemplateSummary();
        bool TemplateItemExists(string description);
        bool TemplateItemExistsExcludingDescription(string description, int templateId);
    }
}
