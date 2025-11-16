using LymmHolidayLets.Application.Interface.Service;
using Microsoft.Extensions.Caching.Memory;
using Stripe.Checkout;

namespace LymmHolidayLets.Application.Service
{
    public sealed class ManageCheckoutSessionService : IManageCheckoutSessionService
    {
        private readonly IMemoryCache _cache;

        private const string SessionKey = "sessions";

        public ManageCheckoutSessionService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IList<CheckoutSession> GetCurrentSessions()
        {
            return !_cache.TryGetValue(SessionKey, out IList<CheckoutSession>? currentSessions) ? new List<CheckoutSession>() : currentSessions ?? new List<CheckoutSession>();
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
                _cache.Remove(SessionKey);
            }
            else
            {
                SetCache(currentSessions);
            }
        }

        public void AddUpdateSessionCache(Session session, DateOnly checkIn, DateOnly checkout)
        {
            if (_cache.TryGetValue(SessionKey, out IList<CheckoutSession>? currentSessions))
            {
                currentSessions?.Add(new CheckoutSession(session.Id, checkIn, checkout));
            }
            else
            {
                currentSessions = new List<CheckoutSession>
                {
                    new(session.Id, checkIn, checkout)
                };
            }

            SetCache(currentSessions);
        }

        private void SetCache(IList<CheckoutSession>? currentSessions)
        {
            if (currentSessions != null)
            {
                _cache.Set(SessionKey, currentSessions, new MemoryCacheEntryOptions()
                    .SetPriority(CacheItemPriority.NeverRemove)
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60)));
            }
        }
    }
}
