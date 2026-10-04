namespace AutoShine.Web.Models;

public class BookingItem
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int ServiceId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public Booking Booking { get; set; } = null!;
    public Service Service { get; set; } = null!;
}