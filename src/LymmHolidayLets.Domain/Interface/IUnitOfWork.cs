namespace LymmHolidayLets.Domain.Interface
{
	//https://balta.io/blog/dapper-unit-of-work-repository-pattern
	public interface IUnitOfWork : IDisposable
	{
		void BeginTransaction();
		void Commit();
		void Rollback();
	}
}
