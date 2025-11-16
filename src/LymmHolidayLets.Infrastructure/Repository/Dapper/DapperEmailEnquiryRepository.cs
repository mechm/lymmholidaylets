using Dapper;
using LymmHolidayLets.Domain.Model.EmailEnquiry.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperEmailEnquiryRepository : RepositoryBase<EmailEnquiry>, IDapperEmailEnquiryRepository
    {
        public DapperEmailEnquiryRepository(DbSession session) : base(session)
        {
        }

        public IEnumerable<EmailEnquiry> GetAll()
        {
            const string procedure = "Email_Enquiry_GetAll";

            try
            {
                IList<EmailEnquiry> emailEnquiries = new List<EmailEnquiry>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                        commandType: CommandType.StoredProcedure);

                foreach (var emailEnquiry in results)
                {
                    emailEnquiries.Add(new EmailEnquiry(emailEnquiry.EmailEnquiryId, emailEnquiry.Name,
                        emailEnquiry.Company, emailEnquiry.EmailAddress, emailEnquiry.TelephoneNo,
                        emailEnquiry.Subject, emailEnquiry.Message, emailEnquiry.DateTimeOfEnquiry));
                }               

                return emailEnquiries;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all email enquiries with the procedure {procedure}", ex);
            }
        }

        public EmailEnquiry? GetById(int id)
        {
            const string procedure = "Email_Enquiry_GetById";

            try
            {
                EmailEnquiry emailEnquiry;

                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                    EmailEnquiryId = id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                emailEnquiry = new EmailEnquiry(result.EmailEnquiryId, result.Name,
                    result.Company, result.EmailAddress, result.TelephoneNo,
                    result.Subject, result.Message, result.DateTimeOfEnquiry);
              

                return emailEnquiry;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a email enquiry with the procedure {procedure}", ex);
            }
        }

        public void Create(EmailEnquiry emailEnquiry)
        {
            const string procedure = "Email_Enquiry_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    emailEnquiry.Name,
                    emailEnquiry.Company,
                    emailEnquiry.EmailAddress,
                    emailEnquiry.TelephoneNo,
                    emailEnquiry.Subject,
                    emailEnquiry.Message,
                    emailEnquiry.DateTimeOfEnquiry
                },
                commandType: CommandType.StoredProcedure);               
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating a email enquiry with the procedure {procedure}", ex);
            }
        }

        public void Update(EmailEnquiry emailEnquiry)
        {
            const string procedure = "Email_Enquiry_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure,
                new
                {
                    emailEnquiry.EmailEnquiryId,
                    emailEnquiry.Name,
                    emailEnquiry.Company,
                    emailEnquiry.EmailAddress,
                    emailEnquiry.TelephoneNo,
                    emailEnquiry.Subject,
                    emailEnquiry.Message,
                    emailEnquiry.DateTimeOfEnquiry
                },
                commandType: CommandType.StoredProcedure);              
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating a email enquiry with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "Email_Enquiry_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    EmailEnquiryId = id
                }, commandType: CommandType.StoredProcedure);                
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a email enquiry with the procedure {procedure}", ex);
            }
        }
    }
}
