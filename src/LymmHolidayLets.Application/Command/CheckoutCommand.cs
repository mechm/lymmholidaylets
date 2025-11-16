using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class CheckoutCommand: ICheckoutCommand
	{
		private readonly IDapperCheckoutRepository _checkoutRepository;
		public CheckoutCommand(IDapperCheckoutRepository checkoutRepository) {
			_checkoutRepository = checkoutRepository;
		}

		public void Create(Checkout checkout)
		{
			_checkoutRepository.Create(
				new Domain.Model.Checkout.Entity.Checkout(checkout.PropertyId, checkout.CheckIn, 
				checkout.CheckOut, checkout.StripeNightProductId, checkout.StripeNightDefaultPriceId,
			    checkout.StripeNightDefaultUnitPrice, checkout.StripeNightCouponId, checkout.StripeNightPercentage,
			    checkout.OverallPrice));
		}

		public void Update(Checkout checkout) 
		{
			_checkoutRepository.Update(
				new Domain.Model.Checkout.Entity.Checkout(checkout.Id, checkout.PropertyId, checkout.CheckIn,
				checkout.CheckOut, checkout.StripeNightProductId, checkout.StripeNightDefaultPriceId,
				checkout.StripeNightDefaultUnitPrice, checkout.StripeNightCouponId, checkout.StripeNightPercentage,
				checkout.OverallPrice));
		}

	}
}
