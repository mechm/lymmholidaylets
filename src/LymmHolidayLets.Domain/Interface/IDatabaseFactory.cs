using System.Data;

namespace LymmHolidayLets.Domain.Interface
{
	public interface IDatabaseFactory
	{
		IDbConnection Get();
	}
}
