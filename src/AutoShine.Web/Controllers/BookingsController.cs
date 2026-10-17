using AutoShine.Web.Data;
using AutoShine.Web.Models;
using AutoShine.Web.Models.Enums;
using AutoShine.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoShine.Web.Controllers;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;
    public BookingsController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var bookings = await _context.Bookings
            .Include(booking => booking.Vehicle)
            .OrderByDescending(booking => booking.ScheduledAt)
            .ToListAsync();
        return View(bookings);
    }

    public async Task<IActionResult> Create()
    {
        var model = new CreateBookingViewModel();

        await PopulateOptionsAsync(model);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookingViewModel model)
    {
        if (ModelState.IsValid)
        {
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == model.VehicleId);

            var service = await _context.Services.FirstOrDefaultAsync(s => s.Id == model.ServiceId && s.IsActive);

            if (vehicle == null || service == null)
            {
                ModelState.AddModelError(nameof(model.VehicleId), "Invalid vehicle or service selected.");
            }

            if (vehicle != null && service != null)
            {
                var unitPrice = service.CurrentPrice;
                var itemTotal = unitPrice * model.Quantity;

                var booking = new Booking
                {
                    VehicleId = model.VehicleId,
                    ScheduledAt = model.ScheduledAt!.Value,
                    Status = BookingStatus.Booked,
                    PaymentStatus = PaymentStatus.Unpaid,
                    Subtotal = itemTotal,
                    DiscountAmount = 0m,
                    TotalAmount = itemTotal,
                    MembershipDiscountPercentage = 0m,
                    RequiredDepositPercentage = 30m,
                    MembershipTypeSnapshot = "Non-member",

                    BookingItems = new List<BookingItem>
                    {
                        new BookingItem
                        {
                            ServiceId = service.Id,
                            ServiceNameSnapshot = service.Name,
                            ServiceTypeSnapshot = service.Type,
                            UnitPrice = unitPrice,
                            Quantity = model.Quantity,
                            Total = itemTotal
                        }
                    }
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        await PopulateOptionsAsync(model);
        return View(model);
    }

    private async Task PopulateOptionsAsync(CreateBookingViewModel model)
    {
        var vehicles = await _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Customer)
            .OrderBy(v => v.PlateNumber)
            .ToListAsync();

        model.Vehicles = vehicles
            .Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = $"{v.PlateNumber} - {v.Brand} {v.Model} ({v.Customer.Name})"
            })
            .ToList();

        var services = await _context.Services
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new
            {
                s.Id,
                s.Name,
                s.CurrentPrice
            })
            .ToListAsync();

        model.Services = services
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.Name} - Rp {s.CurrentPrice:N0}"
            })
            .ToList();
    }
}