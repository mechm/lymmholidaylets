using Dapper;
using LymmHolidayLets.Domain.Model.FAQ.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperFAQRepository : RepositoryBase<FAQ>, IDapperFAQRepository
    {
        public DapperFAQRepository(DbSession session) : base(session)
        {
        }

        public FAQ? GetById(int id)
        {
            const string procedure = "FAQ_GetById";

            try
            {
                FAQ faq;

                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                faq = new FAQ(result.ID, result.PropertyID, result.Question, result.Answer, result.Visible);

                return faq;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a faq with the procedure {procedure}", ex);
            }
        }

        public IEnumerable<FAQ> GetAll()
        {
            const string procedure = "FAQ_GetAll";

            try
            {
                IList<FAQ> faqs = new List<FAQ>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                    commandType: CommandType.StoredProcedure);

                foreach (var faq in results)
                {
                    faqs.Add(new FAQ(faq.ID, faq.PropertyID, faq.Question, faq.Answer, faq.Visible));
                }

                return faqs;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all faqs with the procedure {procedure}", ex);
            }
        }

        public void Create(FAQ faq)
        {
            const string procedure = "FAQ_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                   faq.PropertyID,
                   faq.Question,
                   faq.Answer,
                   faq.Visible
                },
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating a faq with the procedure {procedure}", ex);
            }
        }

        public void Update(FAQ faq)
        {
            const string procedure = "FAQ_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure,
                new
                {
                    faq.ID,
                    faq.PropertyID,
                    faq.Question,
                    faq.Answer,
                    faq.Visible
                },
                commandType: CommandType.StoredProcedure);

            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating a faq with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "FAQ_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    id
                }, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a faq with the procedure {procedure}", ex);
            }
        }
    }
}
