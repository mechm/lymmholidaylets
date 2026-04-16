using System.Data;
using Dapper;

namespace LymmHolidayLets.Infrastructure.Dapper
{
    /// <summary>
    /// Teaches Dapper to use DateOnly as a SQL DATE parameter and to read DATE columns back as DateOnly.
    /// Register once at application startup via SqlMapper.AddTypeHandler(new DateOnlyTypeHandler()).
    /// </summary>
    public sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value.ToDateTime(TimeOnly.MinValue);
        }

        public override DateOnly Parse(object value) => value switch
        {
            DateTime dt => DateOnly.FromDateTime(dt),
            DateOnly d  => d,
            _           => DateOnly.Parse(value.ToString()!)
        };
    }
}

