using LymmHolidayLets.Api.Models.Homepage;

namespace LymmHolidayLets.Api.Services
{
    public interface IHomepageService
    {
        Task<HomepageModel?> GetHomepageDataAsync();
    }
}
