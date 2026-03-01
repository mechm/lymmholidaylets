﻿namespace LymmHolidayLets.Application.Interface.Service
{
	public interface ICheckoutService
	{
		(string? error, Stripe.Checkout.Session? session) Checkout(string host, byte propertyId, DateOnly checkIn, DateOnly checkout, short? numberOfAdults, short? numberOfChildren, short? numberOfInfants, bool available = true);
	}
}
