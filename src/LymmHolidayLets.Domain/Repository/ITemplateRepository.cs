using LymmHolidayLets.Domain.Model.Template;

namespace LymmHolidayLets.Domain.Repository
{
    public interface ITemplateRepository : IRepository<Template>
    {
        Template? GetById(int id);
        IEnumerable<Template> GetAll();
        void Create(Template template);
        void Update(Template template);
        void Delete(int id);
    }
}
