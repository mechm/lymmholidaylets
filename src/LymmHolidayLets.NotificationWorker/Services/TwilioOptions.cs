namespace LymmHolidayLets.NotificationWorker.Services
{
    /// <summary>
    /// Configuration options for the Twilio SMS service.
    /// Bound from the <c>"Twilio"</c> section of <c>appsettings.json</c>.
    /// <c>TwilioClient.Init</c> is called once at startup in <c>Program.cs</c> using
    /// these values so the service itself never needs to touch credentials or reinitialise
    /// the global Twilio client.
    /// </summary>
    public sealed class TwilioOptions
    {
        /// <summary>Twilio Account SID — found in the Twilio Console dashboard.</summary>
        public required string AccountSid { get; init; }

        /// <summary>Twilio Auth Token — found in the Twilio Console dashboard.</summary>
        public required string AuthToken { get; init; }

        /// <summary>
        /// The E.164 phone number to send messages from.
        /// Defaults to the registered Twilio number if not overridden in configuration.
        /// </summary>
        public string FromNumber { get; init; } = "+447897031197";
    }
}

