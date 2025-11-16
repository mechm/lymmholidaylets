using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
	public sealed class BookingCalendarCommand
	{
		private readonly ILogger _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IDapperBookingRepository _bookingRepository;
		private readonly IDapperCalendarRepository _calendarRepository;

		public BookingCalendarCommand(ILogger logger, IUnitOfWork unitOfWork, IDapperBookingRepository bookingRepository, IDapperCalendarRepository calendarRepository)
		{
			_logger = logger;
			_unitOfWork = unitOfWork;
			_bookingRepository = bookingRepository;
			_calendarRepository = calendarRepository;
		}

		public void Create(Booking booking, Calendar calendar)
		{
			//Booking_Calendar_Upsert


			// _unitOfWork.BeginTransaction();

			//try
			//{
			//	_bookingRepository.Create(
			//		new Domain.Model.Booking.Entity.Booking(booking.EventID, booking.SessionID, booking.PropertyID, booking.CheckIn, booking.CheckOut, booking.NoAdult, booking.NoChildren,
			//		booking.NoInfant, booking.Name, booking.Email, booking.Telephone, booking.PostalCode, booking.Country, booking.Total));

			//	_calendarRepository.Update(
			//		new Domain.Model.Calendar.Entity.Calendar(calendar.ID, calendar.PropertyID, calendar.Date, calendar.Price, calendar.MinimumStay, calendar.MaximumStay, calendar.Available, 
			//		calendar.Booked, calendar.BookingID));

			//	_unitOfWork.Commit();
			//}
			//catch (Exception ex)
			//{
			//	_unitOfWork.Rollback();
			//	_logger.LogError("Error with Booking and Calendar Create Transaction", ex);
			//}



			//using (DalSession dalSession = new DalSession())
			//{
			//	UnitOfWork unitOfWork = dalSession.UnitOfWork;
			//	unitOfWork.Begin();
			//	try
			//	{
			//		//Your database code here
			//		MyRepository myRepository = new MyRepository(unitOfWork);
			//		myRepository.Insert(myPoco);
			//		//You may create other repositories in similar way in same scope of UoW.

			//		unitOfWork.Commit();
			//	}
			//	catch
			//	{
			//		unitOfWork.Rollback();
			//		throw;
			//	}
			//}
		}
	}
}
