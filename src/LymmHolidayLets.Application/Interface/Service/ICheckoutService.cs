﻿using LymmHolidayLets.Application.Model.Service;

namespace LymmHolidayLets.Application.Interface.Service
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> CheckoutAsync(
            byte propertyId,
            DateOnly checkIn,
            DateOnly checkout,
            short? numberOfAdults,
            short? numberOfChildren,
            short? numberOfInfants,
            CancellationToken cancellationToken = default);
    }
}


