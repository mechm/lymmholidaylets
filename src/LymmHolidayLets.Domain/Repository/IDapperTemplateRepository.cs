using LymmHolidayLets.Domain.Model.Template;

namespace LymmHolidayLets.Domain.Repository
{
    public interface IDapperTemplateRepository : IDapperRepository<Template>
    {
        Template? GetById(int id);
        IEnumerable<Template> GetAll();
        void Create(Template template);
        void Update(Template template);
        void Delete(int id);
    }
}