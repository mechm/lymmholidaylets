using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class PageCommand : IPageCommand
    {
        private readonly IDapperPageRepository _pageRepository;

        public PageCommand(IDapperPageRepository pageRepository)
        {
            _pageRepository = pageRepository;
        }

        public void Create(Page page)
        {
            _pageRepository.Create(new Domain.Model.Page.Entity.Page(page.AliasTitle,
                page.MetaDescription, page.Title, page.MainImage, page.MainImageAlt, page.Description,
                new Domain.Model.Page.Entity.Template(page.TemplateId, string.Empty),
                page.Visible));
        }

        public void Update(Page page)
        {
            _pageRepository.Update(new Domain.Model.Page.Entity.Page(
                 page.PageId, page.AliasTitle, page.MetaDescription, page.Title, page.MainImage, page.MainImageAlt,
                 page.Description, new Domain.Model.Page.Entity.Template(page.TemplateId,
                 string.Empty), page.Visible));
        }

        public void Delete(int id)
        {
            _pageRepository.Delete(id);
        }
    }
}
