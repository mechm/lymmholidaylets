using LymmHolidayLets.Domain.Interface;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;

namespace LymmHolidayLets.Infrastructure
{
	public sealed class DatabaseFactory(IConfiguration config) : IDatabaseFactory
	{
		private readonly IConfiguration _config = config;

        public IDbConnection Get()
		{
			return new SqlConnection(_config["ConnectionStrings:LymmHolidayLets"]);
		}
	}
}
