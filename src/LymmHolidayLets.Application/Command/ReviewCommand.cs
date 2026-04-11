using LymmHolidayLets.Application.Interface.Command;
using LymmHolidayLets.Application.Interface.Service;
using LymmHolidayLets.Application.Model.Command;
using LymmHolidayLets.Domain.Repository;
using DomainReview = LymmHolidayLets.Domain.Model.Review.Entity.Review;

namespace LymmHolidayLets.Application.Command
{
    public sealed class ReviewCommand(
        IReviewRepository reviewRepository,
        IPropertyCacheInvalidator cacheInvalidator) : IReviewCommand
    {
        public void Create(Review review)
        {
            var reviewToSave = BuildNewReview(review);

            reviewRepository.Create(reviewToSave);

            cacheInvalidator.Invalidate(review.PropertyID);
        }

        public void Create(ref Review review)
        {
            var reviewToSave = BuildNewReview(review);

            reviewRepository.Create(reviewToSave);

            review.RegistrationCode = reviewToSave.RegistrationCode;
            review.DateTimeApproved = reviewToSave.DateTimeApproved;

            cacheInvalidator.Invalidate(review.PropertyID);
        }

        public void Update(Review review)
        {
            var currentReview = reviewRepository.GetById(review.ReviewId);
            if (currentReview == null) 
            {
                return;
            }

            var reviewToSave = BuildUpdatedReview(review, currentReview);

            reviewRepository.Update(reviewToSave);

            cacheInvalidator.Invalidate(review.PropertyID);
        }

        public void Delete(int id)
        {
            var review = reviewRepository.GetById(id);
            reviewRepository.Delete(id);

            if (review is not null)
                cacheInvalidator.Invalidate(review.PropertyID);
        }

        private static DomainReview BuildNewReview(Review review)
        {
            var reviewToSave = new DomainReview(
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
                dateTimeApproved: null,
                approved: false);

            if (review.Approved)
            {
                reviewToSave.Approve(review.DateTimeApproved);
            }

            return reviewToSave;
        }

        private static DomainReview BuildUpdatedReview(Review review, DomainReview currentReview)
        {
            var reviewToSave = new DomainReview(
                review.PropertyID,
                review.ReviewId,
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
                dateTimeApproved: null,
                registrationCode: currentReview.RegistrationCode,
                approved: false,
                created: currentReview.Created);

            if (!review.Approved)
            {
                return reviewToSave;
            }
            
            var approvedAt = currentReview.Approved
                ? currentReview.DateTimeApproved
                : review.DateTimeApproved;

            reviewToSave.Approve(approvedAt);

            return reviewToSave;
        }
    }
}
