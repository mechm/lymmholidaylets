using LymmHolidayLets.Domain.Model.Checkout.Entity;

namespace LymmHolidayLets.Domain.Repository
{
    public interface ICheckoutRepository : IRepository<Checkout>
    {
        Task UpsertAsync(Checkout checkout, CancellationToken cancellationToken = default);
    }
}
