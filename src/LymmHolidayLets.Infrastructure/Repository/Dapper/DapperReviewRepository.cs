using Dapper;
using LymmHolidayLets.Domain.Model.Review.Entity;
using LymmHolidayLets.Domain.Repository;
using LymmHolidayLets.Infrastructure.Exception;
using System.Data;

namespace LymmHolidayLets.Infrastructure.Repository.Dapper
{
    public sealed class DapperReviewRepository : RepositoryBase<Review>, IDapperReviewRepository
    {
        public DapperReviewRepository(DbSession session) : base(session)
        {
        }

        public Review? GetById(int id)
        {
            const string procedure = "Review_GetById";

            try
            {
                Review review;

                using var connection = Session.Connection;
                var result = connection.QueryFirstOrDefault(procedure, new
                {
                   reviewId = id
                },
                commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }
                review = new Review(result.PropertyID, result.ReviewId, result.Company, result.Description, result.PrivateNote, result.Name, result.EmailAddress,
                        result.Position, result.Rating, result.Cleanliness, result.Accuracy,
                     result.Communication, result.Location, result.Checkin, result.Facilities, result.Comfort,
                     result.Value, result.ReviewTypeId, result.LinkToView,
                        result.ShowOnHomepage, result.DateTimeAdded, result.DateTimeApproved, 
                        result.RegistrationCode, result.Approved, result.Created);

                return review;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding a review with the procedure {procedure}", ex);
            }
        }

        public IEnumerable<Review> GetAll()
        {
            const string procedure = "Review_GetAll";

            try
            {
                IList<Review> reviews = new List<Review>();

                using var connection = Session.Connection;
                var results = connection.Query(procedure,
                    commandType: CommandType.StoredProcedure);

                foreach (var result in results)
                {
                    reviews.Add(new Review(result.PropertyID, result.ReviewId, result.Company, result.Description, result.PrivateNote, result.Name, result.EmailAddress,
                        result.Position, result.Rating, result.Cleanliness, result.Accuracy,
                     result.Communication, result.Location, result.Checkin, result.Facilities, result.Comfort,
                     result.Value, result.ReviewTypeId, result.LinkToView,
                        result.ShowOnHomepage, result.DateTimeAdded, result.DateTimeApproved,
                        result.RegistrationCode, result.Approved, result.Created));
                }

                return reviews;
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured finding all reviews with the procedure {procedure}", ex);
            }
        }     

        public void Create(Review review)
        {
            const string procedure = "Review_Insert";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    review.PropertyID,
                    review.Company,
                    review.Description,
                    review.PrivateNote,
                    review.Name,
                    review.EmailAddress,
                    review.Position,
                    review.Rating,
                    review.Cleanliness,
                    review.Accuracy,
                    review.Communication,
                    review.Location,
                    review.Checkin,
                    review.Facilities,
                    review.Comfort,
                    review.Value,
                    review.ReviewTypeId,
                    review.LinkToView,
                    review.ShowOnHomepage,
                    review.DateTimeAdded,
                    review.RegistrationCode,
                    review.DateTimeApproved,
                    review.Approved,
                    review.Created
                },
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured creating a review with the procedure {procedure}", ex);
            }
        }

        public void Update(Review review)
        {
            const string procedure = "Review_Update";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure,
                new
                {
                    review.ReviewId,
                    review.PropertyID,
                    review.Company,
                    review.Description,
                    review.PrivateNote,
                    review.Name,
                    review.EmailAddress,
                    review.Position,
                    review.Rating,
                    review.Cleanliness,
                    review.Accuracy,
                    review.Communication,
                    review.Location,
                    review.Checkin,
                    review.Facilities,
                    review.Comfort,
                    review.Value,
                    review.ReviewTypeId,
                    review.LinkToView,
                    review.ShowOnHomepage,
                    review.DateTimeAdded,
                    review.RegistrationCode,
                    review.DateTimeApproved,
                    review.Approved,
                    review.Created
                },
                commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured updating a review with the procedure {procedure}", ex);
            }
        }

        public void Delete(int id)
        {
            const string procedure = "Review_Delete";

            try
            {
                using var connection = Session.Connection;
                connection.Execute(procedure, new
                {
                    ReviewId = id
                }, commandType: CommandType.StoredProcedure);
            }
            catch (System.Exception ex)
            {
                throw new DataAccessException($"An error occured deleting a review with the procedure {procedure}", ex);
            }
        }
    }
}