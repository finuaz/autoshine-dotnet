using AutoShine.Web.Models.Enums;

namespace AutoShine.Web.Models;

public class BookingStatusHistory
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public BookingStatus? PreviousStatus { get; set; }
    public BookingStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Actor { get; set; }
    public string? Note { get; set; }
    public Booking Booking { get; set; } = null!;
}