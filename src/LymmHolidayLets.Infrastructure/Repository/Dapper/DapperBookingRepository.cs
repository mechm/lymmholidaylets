using Dapper;
using LymmHolidayLets.Domain.Model.Booking.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperBookingRepository : RepositoryBase<Booking>, IDapperBookingRepository
	{
		public DapperBookingRepository(DbSession session) : base(session)
		{
		}

        public Booking? GetById(int id)
        {
            const string procedure = "Booking_GetByID";

            try
            {
                Booking booking;

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

                booking = new Booking(result.ID, result.EventID, result.SessionID, result.PropertyID, result.CheckIn, result.CheckOut,
                                        result.NoAdult, result.NoChildren, result.NoInfant, result.NoOfGuests,
                                        result.Name, result.Email, result.Telephone,
                                        result.PostalCode, result.Country, result.Total, result.Created, result.Updated);

                return booking;
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
                IList<Booking> bookings = new List<Booking>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                    commandType: CommandType.StoredProcedure);

                foreach (var result in results)
                {
                    bookings.Add(new Booking(result.ID, result.EventID, result.SessionID, result.PropertyID, result.CheckIn, result.CheckOut,
                                        result.NoAdult, result.NoChildren, result.NoInfant, result.NoOfGuests,
                                        result.Name, result.Email, result.Telephone,
                                        result.PostalCode, result.Country, result.Total, result.Created, result.Updated));
                }

                return bookings;
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
					booking.CheckIn,
					booking.CheckOut,
					booking.NoAdult,
					booking.NoChildren,
					booking.NoInfant,
					booking.Name,
					booking.Email,
					booking.Telephone,
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
                    booking.CheckIn,
                    booking.CheckOut,
                    booking.NoAdult,
                    booking.NoChildren,
                    booking.NoInfant,
                    booking.Name,
                    booking.Email,
                    booking.Telephone,
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