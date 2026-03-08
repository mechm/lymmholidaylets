using Dapper;
using LymmHolidayLets.Domain.Model.WebhookEvent.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperWebhookEventRepository(DbSession session)
        : RepositoryBase<WebhookEvent>(session), IWebhookEventRepository
    {
        public WebhookEvent? GetByExternalId(string externalId)
        {
            const string procedure = "WebhookEvent_GetByExternalId";
            try
            {
                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new { externalId }, commandType: CommandType.StoredProcedure);
                return result == null ? null : new WebhookEvent(result.Id, result.ExternalId, result.Data, (Domain.Model.WebhookEvent.Enum.WebhookEventState)result.State, result.ProcessingErrors, result.CreatedAt, result.UpdatedAt);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occurred finding a webhook event with the procedure {procedure}", ex);
            }
        }

        public void Create(WebhookEvent webhookEvent)
        {
            const string procedure = "WebhookEvent_Create";
            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    webhookEvent.ExternalId,
                    webhookEvent.Data,
                    State = (int)webhookEvent.State,
                    webhookEvent.CreatedAt,
                    webhookEvent.UpdatedAt
                }, Session.Transaction, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occurred creating a webhook event with the procedure {procedure}", ex);
            }
        }

        public void Update(WebhookEvent webhookEvent)
        {
            const string procedure = "WebhookEvent_Update";
            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    webhookEvent.ExternalId,
                    State = (int)webhookEvent.State,
                    webhookEvent.ProcessingErrors,
                    webhookEvent.UpdatedAt
                }, Session.Transaction, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occurred updating a webhook event with the procedure {procedure}", ex);
            }
        }
    }
}
