using LymmHolidayLets.Application.Interface.Query;
using LymmHolidayLets.Domain.DataAdapter;
using LymmHolidayLets.Domain.Model.Template;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Query
{
    public sealed class TemplateQuery : ITemplateQuery
    {
        private readonly IDapperTemplateDataAdapter _templateDataAdapter;
        private readonly IDapperTemplateRepository _templateRepository;

        public TemplateQuery(IDapperTemplateDataAdapter templateDataAdapter,
            IDapperTemplateRepository templateRepository)
        {
            _templateDataAdapter = templateDataAdapter;
            _templateRepository = templateRepository;
        }      

        public IEnumerable<Template> GetAll()
        {
            return _templateRepository.GetAll();
        }

        public Template GetById(int id)
        {
            return _templateRepository.GetById(id);
        }

        public bool TemplateItemExists(string description)
        {
            return _templateDataAdapter.TemplateItemExists(description);
        }

        public bool TemplateItemExistsExcludingDescription(string description, int templateId)
        {
            return _templateDataAdapter.TemplateItemExistsExcludingDescription(description, templateId);
        }
    }
}
