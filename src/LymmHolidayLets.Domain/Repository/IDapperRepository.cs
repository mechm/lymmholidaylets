using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Domain.Repository
{
	public interface IDapperRepository<T> where T : IAggregateRoot
	{
	}
}
