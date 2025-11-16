using LymmHolidayLets.Domain.Interface;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository
{
	public  sealed class DbSession : IDisposable
	{ 
		public IDbTransaction? Transaction { get; set; }

		public DbSession(IDatabaseFactory databaseFactory)
		{
			DatabaseFactory = databaseFactory;
		}

		public IDatabaseFactory DatabaseFactory
		{
			get; set;
		}

		public IDbConnection Connection => DatabaseFactory.Get();


		public void Dispose() => Connection.Dispose();
	}
}
