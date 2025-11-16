namespace LymmHolidayLets.Domain.ReadModel.Homepage
{
    public sealed class HomepageAggregate
    {
        public HomepageAggregate(IEnumerable<Review> reviews, IEnumerable<Slideshow> slides) 
        {
            Reviews = reviews;
            Slides = slides;
        }

        public IEnumerable<Review> Reviews { get; set; }
        public IEnumerable<Slideshow> Slides { get; set; }
    }
}