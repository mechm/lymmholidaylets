using LymmHolidayLets.Domain.Model.Calendar.Entity;
using LymmHolidayLets.Domain.Model.Property.Entity;
using LymmHolidayLets.Domain.Model.Page.Entity;
using Microsoft.EntityFrameworkCore;

namespace LymmHolidayLets.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<CalendarEF> Calendar { get; set; }
        public DbSet<PropertyEF> Property { get; set; }
        public DbSet<PageEF> Page { get; set; }

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

            modelBuilder.Entity<PropertyEF>(entity =>
            {
                entity.HasKey(p => p.ID);
                entity.Property(p => p.FriendlyName).IsRequired();
                entity.Property(p => p.DefaultNightlyPrice).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Created).IsRequired();
            });

            modelBuilder.Entity<PageEF>(entity =>
            {
                entity.HasKey(p => p.PageId);
                entity.Property(p => p.AliasTitle).IsRequired();
                entity.Property(p => p.MetaDescription).IsRequired();
                entity.Property(p => p.Title).IsRequired();
                entity.Property(p => p.Description).IsRequired().HasColumnName("Description");// Ensure this matches SQL exactly;
                entity.Property(p => p.Visible).IsRequired();
                // TemplateId and TemplateDescription may come from a join in SQL; keep as optional/required as appropriate
                entity.Property(p => p.TemplateId).IsRequired();
                entity.Property(p => p.TemplateDescription).IsRequired(false);
            });
        }
    }
}
