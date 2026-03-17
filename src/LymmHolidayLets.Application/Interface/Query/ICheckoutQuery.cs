﻿using LymmHolidayLets.Domain.ReadModel.Checkout;
namespace LymmHolidayLets.Application.Interface.Query
{
	public interface ICheckoutQuery
	{
		CheckoutLookupResult GetByPropertyIdAndDate(byte propertyId, DateOnly checkIn, DateOnly checkout);
    }
}
