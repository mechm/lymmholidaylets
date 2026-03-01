﻿using LymmHolidayLets.Domain.Model.Checkout.Entity;

namespace LymmHolidayLets.Domain.Repository
{
	public interface IDapperCheckoutRepository : IDapperRepository<Checkout>
	{
		void Upsert(Checkout checkout);
	}
}
