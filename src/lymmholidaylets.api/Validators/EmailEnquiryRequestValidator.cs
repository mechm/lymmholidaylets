using FluentValidation;
using LymmHolidayLets.Api.Models.Email;

namespace LymmHolidayLets.Api.Validators
{
    public sealed class EmailEnquiryRequestValidator : AbstractValidator<EmailEnquiryRequest>
    {
        public EmailEnquiryRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must be a maximum of 100 characters long");

            RuleFor(x => x.Company)
                .MaximumLength(150).WithMessage("Company must be a maximum of 150 characters long");

            RuleFor(x => x.EmailAddress)
                .NotEmpty().WithMessage("Email Address is required")
                .Length(5, 100).WithMessage("Email must be at least 5 characters long and maximum 100 characters long")
                .EmailAddress().WithMessage("Must be a valid email address");

            RuleFor(x => x.TelephoneNo)
                .Length(5, 30).WithMessage("Telephone number must be at least 5 characters long and maximum 30 characters long")
                .Matches(@"^\(?(?:(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?\(?(?:0\)?[\s-]?\(?)?|0)(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}|\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4}|\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3})|\d{5}\)?[\s-]?\d{4,5}|8(?:00[\s-]?11[\s-]?11|45[\s-]?46[\s-]?4\d))(?:(?:[\s-]?(?:x|ext\.?\s?|\#)\d+)?)$")
                .WithMessage("Telephone number must be in the correct format")
                .When(x => !string.IsNullOrEmpty(x.TelephoneNo));

            RuleFor(x => x.Subject)
                .MaximumLength(200);

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .MaximumLength(5000);

            RuleFor(x => x.ReCaptchaToken)
                .NotEmpty().WithMessage("Security verification token is missing");
        }
    }
}
