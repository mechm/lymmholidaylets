using LymmHolidayLets.Domain.Interface;

namespace LymmHolidayLets.Infrastructure.Repository
{
	public abstract class RepositoryBase<T>(DbSession session)
		where T : IAggregateRoot
	{
		protected readonly DbSession Session = session;
	}
}
