using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Infrastructure.Repository;

namespace LymmHolidayLets.Infrastructure
{
	//https://balta.io/blog/dapper-unit-of-work-repository-pattern
	public sealed class UnitOfWork(DbSession session) : IUnitOfWork, IDisposable
	{
		private readonly DbSession _session = session;

        public void BeginTransaction()
		{
			_session.Transaction = _session.Connection.BeginTransaction();
		}

		public void Commit()
		{
			try
			{   
				_session.Transaction?.Commit();
			}
			finally
			{
				Dispose();
			}
		}

		public void Rollback()
		{
			try
			{
				_session.Transaction?.Rollback();
			}
			finally
			{
				Dispose();
			}
		}

		public void Dispose()
		{
			_session.Transaction?.Dispose();
		}
	}
}
