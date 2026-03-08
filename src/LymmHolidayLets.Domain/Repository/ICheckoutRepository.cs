using LymmHolidayLets.Domain.Model.Checkout.Entity;

namespace LymmHolidayLets.Domain.Repository
{
	public interface ICheckoutRepository : IRepository<Checkout>
	{
		void Upsert(Checkout checkout);
	}
}
