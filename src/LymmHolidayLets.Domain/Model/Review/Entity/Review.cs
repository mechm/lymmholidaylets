using LymmHolidayLets.Domain.Interface;
using LymmHolidayLets.Domain.Model.Review.ValueObject;

namespace LymmHolidayLets.Domain.Model.Review.Entity
{
    public sealed class Review: IAggregateRoot
    {
        // create
        public Review(byte propertyID, string? company, string description, string? privateNote, string name, string? emailAddress,
                        string? position, byte rating, byte? cleanliness, byte? accuracy, byte? communication,
                        byte? location, byte? checkin, byte? facilities, byte? comfort, byte? value,
                        byte? reviewTypeId, string? linkToView,
                        bool? showOnHomepage, DateTime? dateTimeAdded, DateTime? dateTimeApproved, bool approved)
        {
            PropertyID = propertyID;
            Company = company;
            Description = description;
            PrivateNote = privateNote;
            Name = name;
            EmailAddress = emailAddress;
            Position = position;
            Ratings = new ReviewRatings(
                rating,
                accuracy,
                cleanliness,
                communication,
                checkin,
                location,
                facilities,
                comfort,
                value);
            ReviewTypeId = reviewTypeId;
            LinkToView = linkToView;
            ShowOnHomepage = showOnHomepage;
            DateTimeAdded = dateTimeAdded;
            DateTimeApproved = dateTimeApproved;
            RegistrationCode = Guid.NewGuid();
            Approved = approved;
            Created = DateTime.UtcNow;
        }

        // update
        public Review(byte reviewId, byte propertyID, string? company, string description, string? privateNote, string name, string? emailAddress,
                        string? position, byte rating, byte? cleanliness, byte? accuracy, byte? communication,
                        byte? location, byte? checkin, byte? facilities, byte? comfort, byte? value, byte? reviewTypeId, string? linkToView,
                        bool? showOnHomepage, DateTime? dateTimeAdded, DateTime? dateTimeApproved, bool approved) : 
            this(propertyID, company, description, privateNote, name, emailAddress,
                        position, rating, cleanliness, accuracy, communication,
                        location, checkin, facilities, comfort, value, reviewTypeId, linkToView,
                        showOnHomepage, dateTimeAdded, dateTimeApproved, approved)
        {
            ReviewId = reviewId;
        }

        // read
        public Review(byte propertyID, byte reviewId, string? company, string description, string? privateNote, string name, string? emailAddress,
                   string? position, byte rating, byte? cleanliness, byte? accuracy, byte? communication,
                         byte? location, byte? checkin, byte? facilities, byte? comfort, byte? value, byte? reviewTypeId, string? linkToView,
                    bool? showOnHomepage, DateTime? dateTimeAdded, DateTime? dateTimeApproved,
                    Guid registrationCode, bool approved, DateTime created) :
                this(propertyID, company, description, privateNote, name, emailAddress,
                   position, rating, cleanliness, accuracy, communication,
                        location, checkin, facilities, comfort, value, reviewTypeId, linkToView,
                   showOnHomepage, dateTimeAdded, dateTimeApproved, approved)
        {
            ReviewId = reviewId;
            RegistrationCode = registrationCode;
            Created = created;
        }

        public void Approve(DateTime? approvedAt = null)
        {
            if (Approved)
            {
                throw new InvalidOperationException("Review is already approved.");
            }

            Approved = true;
            DateTimeApproved = approvedAt ?? DateTime.UtcNow;
        }

        public void Unapprove()
        {
            Approved = false;
            DateTimeApproved = null;
        }

        public byte ReviewId { get; private set; }
        public byte PropertyID { get; private set; }
        public string? Company { get; private set; }
        public string Description { get; private set; }
        public string? PrivateNote { get; private set; }
        public string Name { get; private set; }
        public string? EmailAddress { get; private set; }
        public string? Position { get; private set; }
        public ReviewRatings Ratings { get; private set; }
        public byte Rating => Ratings.Overall;
        public byte? Accuracy => Ratings.Accuracy;
        public byte? Cleanliness => Ratings.Cleanliness;
        public byte? Communication => Ratings.Communication;
        public byte? Location => Ratings.Location;
        public byte? Checkin => Ratings.CheckIn;
        public byte? Facilities => Ratings.Facilities;
        public byte? Comfort => Ratings.Comfort;
        public byte? Value => Ratings.Value;
        public byte? ReviewTypeId { get; private set; }
        public string? LinkToView { get; private set; }
        public bool? ShowOnHomepage { get; private set; }
        public DateTime? DateTimeAdded { get; private set; }
        public DateTime? DateTimeApproved { get; private set; }
        public Guid RegistrationCode { get; private set; }
        public bool Approved { get; private set; }
        public DateTime Created { get; private set; }
    }
}
