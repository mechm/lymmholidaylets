using Dapper;
using LymmHolidayLets.Domain.Model.Booking.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperBookingRepository : RepositoryBase<Booking>, IBookingRepository
	{
		public DapperBookingRepository(DbSession session) : base(session)
		{
		}

        public Booking? GetById(int id)
        {
            const string procedure = "Booking_GetByID";

            try
            {
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

                return new Booking(result.ID, result.EventID, result.SessionID, result.PropertyID, result.CheckIn, result.CheckOut,
                                        result.NoAdult, result.NoChildren, result.NoInfant, result.NoOfGuests,
                                        result.Name, result.Email, result.Telephone,
                                        result.PostalCode, result.Country, result.Total, result.Created, result.Updated);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a booking with the procedure {procedure}", ex);
            }
        }

        public IEnumerable<Booking> GetAll()
        {
            const string procedure = "Booking_GetAll";

            try
            {
                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                    commandType: CommandType.StoredProcedure);

                return results.Select(result => new Booking(result.ID, result.EventID, result.SessionID, result.PropertyID, result.CheckIn, result.CheckOut,
                                        result.NoAdult, result.NoChildren, result.NoInfant, result.NoOfGuests,
                                        result.Name, result.Email, result.Telephone,
                                        result.PostalCode, result.Country, result.Total, result.Created, result.Updated)).ToList();
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all bookings with the procedure {procedure}", ex);
            }
        }

        public void Create(Booking booking)
		{
			const string procedure = "Booking_Calendar_Upsert";

			try
			{
				using var connection = Session.Connection;
				connection.Execute(procedure, new
				{
					booking.SessionID,
					booking.EventID,
					booking.PropertyID,
					CheckIn = booking.Period.CheckIn,
					CheckOut = booking.Period.CheckOut,
					booking.NoAdult,
					booking.NoChildren,
					booking.NoInfant,
					Name = booking.Contact.Name,
					Email = booking.Contact.Email,
					Telephone = booking.Contact.Telephone,
					booking.PostalCode,
					booking.Country,
					booking.Total,
					booking.Created
				},
				Session.Transaction,
				commandType: CommandType.StoredProcedure);
			}
			catch (System.Exception ex)
			{
				throw new DataAccessException($"An error occurred creating a booking with the procedure {procedure}", ex);
			}
		}

        public void Update(Booking booking)
        {
            const string procedure = "Booking_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    booking.ID,
                    booking.SessionID,
                    booking.EventID,
                    booking.PropertyID,
                    CheckIn = booking.Period.CheckIn,
                    CheckOut = booking.Period.CheckOut,
                    booking.NoAdult,
                    booking.NoChildren,
                    booking.NoInfant,
                    Name = booking.Contact.Name,
                    Email = booking.Contact.Email,
                    Telephone = booking.Contact.Telephone,
                    booking.PostalCode,
                    booking.Country,
                    booking.Total,
                    booking.Updated
                },
                Session.Transaction,
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occurred updating a booking with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "Booking_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    id
                }, commandType: CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a booking with the procedure {procedure}", ex);
            }
        }
    }
}
