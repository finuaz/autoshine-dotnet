using AutoShine.Web.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AutoShine.Web.Models;

public class Service
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Service name is required.")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    [Required(ErrorMessage = "Service type is required.")]
    public ServiceType Type { get; set; }

    [Range(typeof(decimal), "0", "78968765465465651651564654768.99", ErrorMessage = "Price must be a positive number.")]
    public decimal CurrentPrice { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Estimated minutes must be a positive number.")]
    public int EstimatedMinutes { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<BookingItem> BookingItems { get; set; } = new List<BookingItem>();
}