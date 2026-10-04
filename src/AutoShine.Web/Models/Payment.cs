namespace AutoShine.Web.Models;

public class Payment
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public decimal Amount { get;set; }
    public DateTime PaidAt { get; set; }
    public string? Note { get; set; }
    public Booking Booking { get; set; } = null!;
}