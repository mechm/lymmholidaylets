using LymmHolidayLets.Application.Service;

namespace LymmHolidayLets.Application.Interface.Service
{
    public interface IManageCheckoutSessionService
    {
        IList<CheckoutSession> GetCurrentSessions();
        IList<CheckoutSession> GetSessionsBasedOnDates(IList<CheckoutSession> currentSessions, DateOnly checkIn, DateOnly checkout);
        void UpdateSessionCache(IList<CheckoutSession> currentSessions);
        void AddUpdateSessionCache(string sessionId, DateOnly checkIn, DateOnly checkout);
    }
}


