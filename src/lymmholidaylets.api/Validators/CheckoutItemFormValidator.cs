using FluentValidation;
using LymmHolidayLets.Api.Models.Checkout;

namespace LymmHolidayLets.Api.Validators
{
    public sealed class CheckoutItemFormValidator : AbstractValidator<CheckoutItemForm>
    {
        public CheckoutItemFormValidator()
        {
            RuleFor(x => x.PropertyId)
                .NotEmpty()
                .GreaterThan((byte)0);

            RuleFor(x => x.Checkin)
                .NotEmpty()
                .Must(date => date >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("Check-in date cannot be in the past.");

            RuleFor(x => x.Checkout)
                .NotEmpty()
                .GreaterThan(x => x.Checkin)
                .WithMessage("Check-out date must be after the check-in date.");

            RuleFor(x => x.NumberOfAdults)
                .InclusiveBetween((short)1, (short)20)
                .WithMessage("Number of adults must be between 1 and 20.");

            RuleFor(x => x.NumberOfChildren)
                .GreaterThanOrEqualTo((short)0);

            RuleFor(x => x.NumberOfInfants)
                .GreaterThanOrEqualTo((short)0);
        }
    }
}
