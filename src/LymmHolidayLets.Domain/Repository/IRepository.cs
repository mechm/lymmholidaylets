using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Repository
{
	public interface IRepository<T> where T : IAggregateRoot
	{
	}
}
