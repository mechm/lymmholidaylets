using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Domain.Dto.Email;
using LymmHolidayLets.Domain.Interface;
using Microsoft.Extensions.Configuration;

namespace LymmHolidayLets.Application.Service
{
	public sealed class EmailGeneratorService(IConfiguration config, IEmailTemplateBuilder emailTemplateBuilder, IEmailService emailService) : IEmailGeneratorService
    {
        private readonly IConfiguration _config = config;
   
        private readonly IEmailTemplateBuilder _emailTemplateBuilder = emailTemplateBuilder;
        private readonly IEmailService _emailService = emailService;
        private readonly IDictionary<string, string?>? _ccEmails = config.GetSection("Keys:CCEmail")
              .AsEnumerable()
              .Where(x => !string.IsNullOrWhiteSpace(x.Value))
              .ToDictionary(x => x.Key.Replace("Keys:CCEmail:", ""), x => x.Value);

        public async Task EmailBookingConfirmationToCompany(BookingConfirmationForCompany bookingConfirmationForCompany)
        {
            string html = await _emailTemplateBuilder.BuildHtmlBookingEmailToCompany(bookingConfirmationForCompany);

            await _emailService.SendAsync(
                new EmailMessage
                {
                    ToName = "Lymm Holiday Lets Booking Confirmation",
                    CcEmailAddress = _ccEmails,
                    ToEmailAddress = _config["Keys:CompanyEmail"],
                    Subject = "Lymm Holiday Lets Booking Confirmation"
                }, html);
        }

        public async Task EmailBookingConfirmationToCustomer(BookingConfirmationForCustomer bookingConfirmationForCustomer)
        {
            string html = await _emailTemplateBuilder.BuildHtmlBookingEmailToCustomer(bookingConfirmationForCustomer);

            await _emailService.SendAsync(
                new EmailMessage
                {
                    ToName = bookingConfirmationForCustomer.Name,
                    ToEmailAddress = bookingConfirmationForCustomer.Email,
                    Subject = $"Reservation confirmed for {bookingConfirmationForCustomer.PropertyName}"
                }, html);
        }
    }
}