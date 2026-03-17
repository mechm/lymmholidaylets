using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface ICheckoutCommand
    {
        Task UpsertAsync(Checkout checkout, CancellationToken cancellationToken = default);
    }
}
