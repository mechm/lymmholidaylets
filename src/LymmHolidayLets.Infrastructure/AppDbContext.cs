using LymmHolidayLets.Domain.Model.Calendar.Entity;
using Microsoft.EntityFrameworkCore;

namespace LymmHolidayLets.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Calendar> Calendars { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Book>(entity =>
        //    {
        //        entity.HasKey(b => b.Id);
        //        entity.Property(b => b.Title).IsRequired().HasMaxLength(200);
        //        entity.Property(b => b.Author).HasMaxLength(100);
        //    });
        //}
    }
}
