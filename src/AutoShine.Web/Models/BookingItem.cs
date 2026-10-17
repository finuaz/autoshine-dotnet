using System.ComponentModel.DataAnnotations;
using AutoShine.Web.Models.Enums;

namespace AutoShine.Web.Models;

public class BookingItem
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int ServiceId { get; set; }

    [Required]
    public string ServiceNameSnapshot { get; set; } = null!;
    public ServiceType ServiceTypeSnapshot { get; set; }
    public decimal UnitPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
    public decimal Total { get; set; }
    public Booking Booking { get; set; } = null!;
    public Service Service { get; set; } = null!;
}