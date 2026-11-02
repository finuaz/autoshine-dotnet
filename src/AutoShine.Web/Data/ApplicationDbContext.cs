using AutoShine.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoShine.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }
    
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<MembershipPlan> MembershipPlans { get; set; }
    public DbSet<CustomerMembership> CustomerMemberships { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<BookingItem> BookingItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<BookingStatusHistory> BookingStatusHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // A vehicle's plate number must be unique.
        modelBuilder.Entity<Vehicle>()
            .HasIndex(v => v.PlateNumber)
            .IsUnique();

        // A customer cannot be deleted while they own vehicles.
        modelBuilder.Entity<Vehicle>()
            .HasOne(v => v.Customer)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        // A vehicle cannot be deleted while it has booking history.
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Vehicle)
            .WithMany(v => v.Bookings)
            .HasForeignKey(b => b.VehicleId)
            .OnDelete(DeleteBehavior.Restrict);

        // A booking cannot be deleted while it has booking items.
        modelBuilder.Entity<BookingItem>()
            .HasOne(item => item.Booking)
            .WithMany(booking => booking.BookingItems)
            .HasForeignKey(item => item.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // A booking cannot be deleted while it has payment records.
        modelBuilder.Entity<Payment>()
            .HasOne(payment => payment.Booking)
            .WithMany(booking => booking.Payments)
            .HasForeignKey(payment => payment.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // A booking cannot be deleted while it has status history.
        modelBuilder.Entity<BookingStatusHistory>()
            .HasOne(history => history.Booking)
            .WithMany(booking => booking.StatusHistories)
            .HasForeignKey(history => history.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        // A service cannot be deleted while historical booking items reference it.
        modelBuilder.Entity<BookingItem>()
            .HasOne(item => item.Service)
            .WithMany(service => service.BookingItems)
            .HasForeignKey(item => item.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}