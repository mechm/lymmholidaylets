using LymmHolidayLets.Domain.Model.Template;

namespace LymmHolidayLets.Application.Interface.Query
{
    public interface ITemplateQuery
    {
        IEnumerable<Template> GetAll();
        Template GetById(int id);
        bool TemplateItemExists(string description);
        bool TemplateItemExistsExcludingDescription(string description, int templateId);
    }
}
