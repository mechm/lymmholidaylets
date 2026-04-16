using System.Data;
using Dapper;

namespace LymmHolidayLets.Infrastructure.Dapper
{
    /// <summary>
    /// Teaches Dapper to convert SQL TIME columns (which arrive as TimeSpan) into TimeOnly.
    /// Register once at application startup via SqlMapper.AddTypeHandler(new TimeOnlyTypeHandler()).
    /// </summary>
    public sealed class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
    {
        public override void SetValue(IDbDataParameter parameter, TimeOnly value)
        {
            parameter.DbType = DbType.Time;
            parameter.Value = value.ToTimeSpan();
        }

        public override TimeOnly Parse(object value) => value switch
        {
            TimeSpan ts => TimeOnly.FromTimeSpan(ts),
            TimeOnly to => to,
            _ => TimeOnly.Parse(value.ToString()!)
        };
    }
}

