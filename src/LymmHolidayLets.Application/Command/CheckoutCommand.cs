using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class CheckoutCommand(ICheckoutRepository checkoutRepository) : ICheckoutCommand
    {
        public Task UpsertAsync(Checkout checkout, CancellationToken cancellationToken = default)
        {
            return checkoutRepository.UpsertAsync(
                new Domain.Model.Checkout.Entity.Checkout(
                    checkout.PropertyId,
                    checkout.CheckIn,
                    checkout.CheckOut,
                    checkout.StripeNightProductId,
                    checkout.StripeNightDefaultPriceId,
                    checkout.StripeNightDefaultUnitPrice,
                    checkout.StripeNightCouponId,
                    checkout.StripeNightPercentage,
                    checkout.OverallPrice),
                cancellationToken);
        }
    }
}
