namespace LymmHolidayLets.Application.Model.Command
{
    public sealed class WebhookEvent
    {
        public WebhookEvent(string externalId, string data, int state, string? processingErrors = null)
        {
            ExternalId = externalId;
            Data = data;
            State = state;
            ProcessingErrors = processingErrors;
        }

        public string ExternalId { get; init; }
        public string Data { get; init; }
        public int State { get; init; }
        public string? ProcessingErrors { get; init; }
    }
}
