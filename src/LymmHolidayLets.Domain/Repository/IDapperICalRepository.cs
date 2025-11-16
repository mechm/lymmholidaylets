using LymmHolidayLets.Domain.Model.ICal.Entity;


namespace LymmHolidayLets.Domain.Repository
{
    public interface IDapperICalRepository : IDapperRepository<ICal>
    {
        IList<ICal> GetAll();
    }
}
