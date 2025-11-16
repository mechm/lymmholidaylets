using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;

namespace LymmHolidayLets.Application.Command
{
    public sealed class ReviewCommand : IReviewCommand
    {
        private readonly IDapperReviewRepository _reviewRepository;

        public ReviewCommand(IDapperReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public void Create(Review review)
        {
            if (review.Approved)
            {
                review.DateTimeApproved = DateTime.UtcNow;
            }

            _reviewRepository.Create(new
                Domain.Model.Review.Entity.Review(review.PropertyID, review.Company,
                    review.Description, review.PrivateNote, review.Name,
                    review.EmailAddress, review.Position,
                    review.Rating, review.Cleanliness, review.Accuracy, review.Communication, 
                    review.Location, review.Checkin, review.Facilities, review.Comfort, review.Value, review.ReviewTypeId,
                    review.LinkToView, review.ShowOnHomepage,
                    review.DateTimeAdded, review.DateTimeApproved,
                    review.Approved));
        }

        public void Create(ref Review review)
        {
            var reviewToSave = new
                Domain.Model.Review.Entity.Review(review.PropertyID, review.Company,
                    review.Description, review.PrivateNote, review.Name,
                    review.EmailAddress, review.Position,
                    review.Rating, review.Cleanliness, review.Accuracy, review.Communication,
                    review.Location, review.Checkin, review.Facilities, review.Comfort, review.Value, review.ReviewTypeId,
                    review.LinkToView, review.ShowOnHomepage,
                    review.DateTimeAdded, review.DateTimeApproved,
                    review.Approved);

            _reviewRepository.Create(reviewToSave);

            review.RegistrationCode = reviewToSave.RegistrationCode;
        }

        public void Update(Review review)
        {
            var currentReview = _reviewRepository.GetById(review.ReviewId);
            if (currentReview == null) 
            {
                return;
            }

            if ((currentReview.Approved == false || review.DateTimeApproved == null) && review.Approved)
            {
                review.DateTimeApproved = DateTime.UtcNow;
            }
            else if (review.Approved == false)
            {
                review.DateTimeApproved = null;
            }

            _reviewRepository.Update(new
                Domain.Model.Review.Entity.Review(review.ReviewId, 
                    review.PropertyID, review.Company,
                    review.Description, review.PrivateNote, review.Name,
                    review.EmailAddress, review.Position,
                    review.Rating, review.Cleanliness, review.Accuracy, review.Communication,
                    review.Location, review.Checkin, review.Facilities, review.Comfort, review.Value, review.ReviewTypeId,
                    review.LinkToView, review.ShowOnHomepage,
                    review.DateTimeAdded, review.DateTimeApproved,
                    review.Approved));
        }

        public void Delete(int id)
        {
            _reviewRepository.Delete(id);
        }
    }
}