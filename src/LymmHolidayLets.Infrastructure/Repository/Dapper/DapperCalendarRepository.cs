using Dapper;
using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperCalendarRepository : RepositoryBase<Calendar>, IDapperCalendarRepository
	{
		public DapperCalendarRepository(DbSession session) : base(session)
		{
		}

        public Calendar? GetById(int id)
        {
            const string procedure = "Calendar_GetById";

            try
            {
                Calendar calendar;

                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                calendar = new Calendar(result.ID, result.PropertyID, result.Date, result.Price,
                        Convert.ToByte(result.MinimumStay),
                        result.MaximumStay != null ? Convert.ToByte(result.MaximumStay) : result.MaximumStay,
                        result.Available, result.Booked, result.BookingID);

                return calendar;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a calendar with the procedure {procedure}", ex);
            }
        }

        public IEnumerable<Calendar> GetByPropertyIDDate(byte propertyId, DateOnly startDate, DateOnly endDate)         
        {
            const string procedure = "Calendar_GetByPropertyID";

            try
            {
                IList<Calendar> availability = new List<Calendar>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure, new
                {
                    propertyId,
                    startDate,
                    endDate,
                },
                Session.Transaction,
                commandType: CommandType.StoredProcedure);

                foreach (var result in results)
                {
                    availability.Add(new Calendar(result.ID, result.PropertyID, result.Date, result.Price,
                        Convert.ToByte(result.MinimumStay),
                        result.MaximumStay != null ? Convert.ToByte(result.MaximumStay) : result.MaximumStay,
                        result.Available, result.Booked, result.BookingID));
                }

                return availability;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException(
                    $"An error occurred finding calendar available with the procedure {procedure}", ex);
            }
        } 

        public void Create(Calendar calendar)
		{
			const string procedure = "Calendar_Insert";

			try
			{
				using var connection = Session.Connection;
				connection.Execute(procedure, new
				{
					calendar.PropertyID,
					calendar.Date,
					calendar.Price,
					calendar.MinimumStay,
					calendar.MaximumStay,
					calendar.Available,
					calendar.Booked,
					calendar.BookingID
				},
				Session.Transaction,
				commandType: CommandType.StoredProcedure);
			}
			catch (System.Exception ex)
			{
				throw new DataAccessException($"An error occurred creating a calendar with the procedure {procedure}", ex);
			}
		}

        public void Update(Calendar calendar)
		{
			const string procedure = "Calendar_Update";

			try
			{
				using var connection = Session.Connection;
				connection.Execute(procedure, new
				{
					calendar.ID,
					calendar.PropertyID,
					calendar.Date,
					calendar.Price,
					calendar.MinimumStay,
					calendar.MaximumStay,
					calendar.Available,
					calendar.Booked,
					calendar.BookingID
				},
				Session.Transaction,
				commandType: CommandType.StoredProcedure);
			}
			catch (System.Exception ex)
			{
				throw new DataAccessException($"An error occurred updating a calendar with the procedure {procedure}", ex);
			}
        }

        public void Update(IEnumerable<int> ids, Calendar calendar)
        {
            const string procedure = "Calendar_Update_Multiple_ID";

            try
            {
                var IDTable = new DataTable();
                IDTable.Columns.Add("IDTable", typeof(int));

                foreach (var id in ids)
                {
                    var row = IDTable.NewRow();
                    row[0] = id;
                    IDTable.Rows.Add(row);
                }

                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    calendar.Price,
                    calendar.MinimumStay,
                    calendar.MaximumStay,
                    calendar.Available,
                    calendar.Booked,
                    calendar.BookingID,
                    IDTable
                },
                Session.Transaction,
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating calendars with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "Calendar_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    CalendarId = id
                }, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a calendar with the procedure {procedure}", ex);
            }
        }
    }
}
