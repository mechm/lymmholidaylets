using LymmHolidayLets.Application.Interface.Service;

namespace LymmHolidayLets.Application.Service
{
    public sealed class ManageCheckoutSessionService(IApplicationCache cache) : IManageCheckoutSessionService
    {
        private const string SessionKey = "sessions";

        public IList<CheckoutSession> GetCurrentSessions()
        {
            return !cache.TryGetValue(SessionKey, out IList<CheckoutSession>? currentSessions) ? new List<CheckoutSession>() : currentSessions ?? new List<CheckoutSession>();
        }

        public IList<CheckoutSession> GetSessionsBasedOnDates(IList<CheckoutSession> currentSessions, 
            DateOnly checkIn, DateOnly checkout)
        {
            return currentSessions.Where(
                session => session.CheckIn <= checkIn && session.Checkout >= checkout ||
                           session.CheckIn >= checkIn && session.Checkout <= checkout).ToList();
        }
        
        public void UpdateSessionCache(IList<CheckoutSession> currentSessions)
        {
            if (currentSessions.Count == 0)
            {
                cache.Remove(SessionKey);
            }
            else
            {
                SetCache(currentSessions);
            }
        }

        public void AddUpdateSessionCache(string sessionId, DateOnly checkIn, DateOnly checkout)
        {
            if (cache.TryGetValue(SessionKey, out IList<CheckoutSession>? currentSessions))
            {
                currentSessions?.Add(new CheckoutSession(sessionId, checkIn, checkout));
            }
            else
            {
                currentSessions = new List<CheckoutSession>
                {
                    new(sessionId, checkIn, checkout)
                };
            }

            SetCache(currentSessions);
        }

        private void SetCache(IList<CheckoutSession>? currentSessions)
        {
            if (currentSessions != null)
            {
                cache.SetSliding(SessionKey, currentSessions, TimeSpan.FromMinutes(60));
            }
        }
    }
}
