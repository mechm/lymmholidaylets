using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Template;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class TemplateQuery(
        IDapperTemplateDataAdapter templateDataAdapter,
        ITemplateRepository templateRepository)
        : ITemplateQuery
    {
        public IEnumerable<Template> GetAll()
        {
            return templateRepository.GetAll();
        }

        public Template? GetById(int id)
        {
            return templateRepository.GetById(id);
        }

        public bool TemplateItemExists(string description)
        {
            return templateDataAdapter.TemplateItemExists(description);
        }

        public bool TemplateItemExistsExcludingDescription(string description, int templateId)
        {
            return templateDataAdapter.TemplateItemExistsExcludingDescription(description, templateId);
        }
    }
}
