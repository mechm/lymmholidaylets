﻿using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface ICheckoutCommand
	{
		void Upsert(Checkout checkout);
	}
}
