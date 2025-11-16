using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Infrastructure.Repository
{
	public abstract class RepositoryBase<T> where T : IAggregateRoot
	{
		protected readonly DbSession Session;
		protected RepositoryBase(DbSession session)
		{
			Session = session;
		}
	}
}
