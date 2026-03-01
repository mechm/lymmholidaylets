﻿using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class CheckoutCommand(IDapperCheckoutRepository checkoutRepository) : ICheckoutCommand
    {
	    public void Upsert(Checkout checkout)
		{
			checkoutRepository.Upsert(
				new Domain.Model.Checkout.Entity.Checkout(
					checkout.PropertyId,
					checkout.CheckIn,
					checkout.CheckOut,
					checkout.StripeNightProductId,
					checkout.StripeNightDefaultPriceId,
					checkout.StripeNightDefaultUnitPrice,
					checkout.StripeNightCouponId,
					checkout.StripeNightPercentage,
					checkout.OverallPrice));
		}
	}
}
