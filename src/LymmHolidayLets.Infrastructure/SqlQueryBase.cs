using Dapper;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Infrastructure.Repository;
using System.Data;

namespace LymmHolidayLets.Infrastructure
{
	public abstract class SqlQueryBase
	{
		protected readonly DbSession _session;
		protected SqlQueryBase(DbSession session)
		{
			_session = session;
		}

        protected IEnumerable<T> GetAll<T>(string tableName) where T : ITable
        {
            IEnumerable<T> items;

            using var connection = _session.Connection;
            connection.Open();          
            items = connection.Query<T>("SELECT * FROM " + tableName + " (NOLOCK)");            

            return items;
        }

        public IEnumerable<T> QueryProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.Query<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public Task<IEnumerable<T>> QueryProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QueryAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T QueryFirstProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QueryFirst<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public Task<T> QueryFirstProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QueryFirstAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T QueryFirstOrDefaultProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QueryFirstOrDefault<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public Task<T> QueryFirstOrDefaultProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QueryFirstOrDefaultAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T QuerySingleProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QuerySingle<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public Task<T> QuerySingleProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			return connection.QuerySingleAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public T QuerySingleOrDefaultProcedure<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QuerySingleOrDefault<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}

		public Task<T> QuerySingleOrDefaultProcedureAsync<T>(string query, DynamicParameters parameters)
		{
			using var connection = _session.Connection;
			connection.Open();
			return connection.QuerySingleOrDefaultAsync<T>(query, parameters, commandType: CommandType.StoredProcedure);
		}
	}
}
