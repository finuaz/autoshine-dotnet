using System.ComponentModel.DataAnnotations;

namespace AutoShine.Web.ViewModels
{
    public class EditBookingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Scheduled date and time is required.")]
        public DateTime? ScheduledAt { get; set; }

    }
}
