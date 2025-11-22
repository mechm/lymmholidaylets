using LymmHolidayLets.Domain.Model.Calendar.Entity;
using Microsoft.EntityFrameworkCore;

namespace LymmHolidayLets.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<CalendarEF> Calendar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CalendarEF>(entity =>
            {
                entity.HasKey(b => b.ID);
                entity.Property(b => b.PropertyID).IsRequired();
                entity.Property(b => b.Date).IsRequired();
                entity.Property(b => b.Price).HasColumnType("decimal(18,2)");
                entity.Property(b => b.MinimumStay).IsRequired();
                entity.Property(b => b.MaximumStay).IsRequired(false);
                entity.Property(b => b.Available).IsRequired();
                entity.Property(b => b.Booked).IsRequired();
                entity.Property(b => b.BookingID).IsRequired(false);
            });
        }
    }
}
