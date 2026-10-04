using AutoShine.Web.Models.Enums;

namespace AutoShine.Web.Models;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ServiceType Type { get; set; }
    public decimal CurrentPrice { get; set; }
    public int EstimatedMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
}