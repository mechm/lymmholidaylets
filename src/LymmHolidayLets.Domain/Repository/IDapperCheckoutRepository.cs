using LymmHolidayLets.Domain.Model.Checkout.Entity;

namespace LymmHolidayLets.Domain.Repository
{
	public interface IDapperCheckoutRepository : IDapperRepository<Checkout>
	{
		void Create(Checkout checkout);
		void Update(Checkout checkout);
	}
}
