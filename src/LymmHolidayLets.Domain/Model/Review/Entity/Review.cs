using LymmHolidayLets.Domain.Interface;

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
            Rating = rating;
            Cleanliness = cleanliness;
            Accuracy = accuracy;
            Communication = communication;
            Location = location;
            Checkin = checkin;
            Facilities = facilities;
            Comfort = comfort;
            Value = value;
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

        public byte ReviewId { get; set; }
        public byte PropertyID { get; set; }
        public string? Company { get; set; }
        public string Description { get; set; }
        public string? PrivateNote { get; set; }
        public string Name { get; set; }    
        public string? EmailAddress { get; set; }
        public string? Position { get; set; }
        public byte Rating { get; set; }
        public byte? Cleanliness { get; set; }
        public byte? Accuracy { get; set; }
        public byte? Communication { get; set; }
        public byte? Location { get; set; }
        public byte? Checkin { get; set; }
        public byte? Facilities { get; set; }
        public byte? Comfort { get; set; }
        public byte? Value { get; set; }
        public byte? ReviewTypeId { get; set; }
        public string? LinkToView { get; set; }
        public bool? ShowOnHomepage { get; set; }
        public DateTime? DateTimeAdded { get; set; }
        public DateTime? DateTimeApproved { get; set; }
        public Guid RegistrationCode { get; set; }
        public bool Approved { get; set; }
        public DateTime Created { get; set; }
    }
}
