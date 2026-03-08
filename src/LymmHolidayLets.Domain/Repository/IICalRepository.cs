using LymmHolidayLets.Domain.Model.ICal.Entity;


namespace LymmHolidayLets.Domain.Repository
{
    public interface IICalRepository : IRepository<ICal>
    {
        IList<ICal> GetAll();
        Task<IReadOnlyList<ICal>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
