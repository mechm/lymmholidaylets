using Dapper;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure
{
	public abstract class SqlQueryBase(DbSession session)
	{
		protected readonly DbSession Session = session;

		protected IEnumerable<T> GetAll<T>(string tableName) where T : ITable
        {
	        using var connection = Session.Connection;
	        connection.Open();          
	        var items = connection.Query<T>("SELECT * FROM " + tableName + " (NOLOCK)");            

            return items;
        }

        public IEnumerable<T> QueryProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return connection.Query<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public async Task<IEnumerable<T>> QueryProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return await connection.QueryAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T QueryFirstProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return connection.QueryFirst<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public async Task<T> QueryFirstProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return await connection.QueryFirstAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T? QueryFirstOrDefaultProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return connection.QueryFirstOrDefault<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public async Task<T?> QueryFirstOrDefaultProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return await connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T QuerySingleProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return connection.QuerySingle<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public async Task<T> QuerySingleProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			return await connection.QuerySingleAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T? QuerySingleOrDefaultProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return connection.QuerySingleOrDefault<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public async Task<T?> QuerySingleOrDefaultProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = Session.Connection;
			connection.Open();
			return await connection.QuerySingleOrDefaultAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}
	}
}
