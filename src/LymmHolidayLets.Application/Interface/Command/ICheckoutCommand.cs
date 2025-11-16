using LymmHolidayLets.Application.Model.Command;

namespace LymmHolidayLets.Application.Interface.Command
{
    public interface ICheckoutCommand
	{
		void Create(Checkout checkout);
		void Update(Checkout checkout);
	}
}
