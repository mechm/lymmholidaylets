using Dapper;

namespace LymmHolidayLets.Domain.Interface
{
	public interface IDapperSqlQueryBase
	{
		IEnumerable<T> QueryProcedure<T>(string query, DynamicParameters parameters);
		Task<IEnumerable<T>> QueryProcedureAsync<T>(string query, DynamicParameters parameters);
		T QueryFirstProcedure<T>(string query, DynamicParameters parameters);
		Task<T> QueryFirstProcedureAsync<T>(string query, DynamicParameters parameters);
		T QueryFirstOrDefaultProcedure<T>(string query, DynamicParameters parameters);
		Task<T> QueryFirstOrDefaultProcedureAsync<T>(string query, DynamicParameters parameters);
		T QuerySingleProcedure<T>(string query, DynamicParameters parameters);
		Task<T> QuerySingleProcedureAsync<T>(string query, DynamicParameters parameters);
		T QuerySingleOrDefaultProcedure<T>(string query, DynamicParameters parameters);
		Task<T> QuerySingleOrDefaultProcedureAsync<T>(string query, DynamicParameters parameters);
	}
}
