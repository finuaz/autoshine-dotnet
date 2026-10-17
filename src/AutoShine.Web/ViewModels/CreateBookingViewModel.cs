using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace AutoShine.Web.ViewModels
{
    public class CreateBookingViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a vehicle.")]
        public int VehicleId { get; set; }

        [Required(ErrorMessage = "Please select a date and time for your booking.")]
        public DateTime? ScheduledAt { get; set; } = DateTime.UtcNow.AddHours(1); // Default to 1 hour from now
        
        [Range(1, int.MaxValue, ErrorMessage = "Please select a service.")]
        public int ServiceId { get; set; }
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100.")]
        public int Quantity { get; set; } = 1; // Default quantity to 1

        [BindNever]
        public List<SelectListItem> Vehicles { get; set; } = new List<SelectListItem>();

        [BindNever]
        public List<SelectListItem> Services { get; set; } = new List<SelectListItem>();
    }
}
