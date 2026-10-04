using AutoShine.Web.Models.Enums;

namespace AutoShine.Web.Models;

public class Booking
{
    public int Id { get; set; }
    public int VehicleId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Booked;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid ;
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal MembershipDiscountPercentage { get; set; }
    public decimal RequiredDepositPercentage { get; set; }
    public string? MembershipTypeSnapshot { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Vehicle Vehicle { get; set; } = null!;
    public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<BookingStatusHistory> StatusHistories { get; set; } = new List<BookingStatusHistory>();
}