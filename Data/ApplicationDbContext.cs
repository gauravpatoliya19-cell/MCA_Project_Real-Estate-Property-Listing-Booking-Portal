using Microsoft.EntityFrameworkCore;
using RealEstatePortal.Web.Models;

namespace RealEstatePortal.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<PropertyImage> PropertyImages { get; set; } = null!;
        public DbSet<PropertyFeature> PropertyFeatures { get; set; } = null!;
        public DbSet<SiteVisitBooking> SiteVisitBookings { get; set; } = null!;
        public DbSet<TokenPayment> TokenPayments { get; set; } = null!;
        public DbSet<RentalContract> RentalContracts { get; set; } = null!;
        public DbSet<Inquiry> Inquiries { get; set; } = null!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Property to PropertyImage (Cascade)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property to PropertyFeature (Cascade)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Features)
                .WithOne(f => f.Property)
                .HasForeignKey(f => f.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property to SiteVisitBooking (Restrict/Cascade)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.SiteVisits)
                .WithOne(b => b.Property)
                .HasForeignKey(b => b.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property to TokenPayment (Restrict/Cascade)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.TokenPayments)
                .WithOne(t => t.Property)
                .HasForeignKey(t => t.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property to RentalContract (Restrict/Cascade)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.RentalContracts)
                .WithOne(c => c.Property)
                .HasForeignKey(c => c.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Property to Inquiry (Cascade)
            modelBuilder.Entity<Property>()
                .HasMany(p => p.Inquiries)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
