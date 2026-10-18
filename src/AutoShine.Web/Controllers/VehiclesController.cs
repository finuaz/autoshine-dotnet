using AutoShine.Web.Data;
using AutoShine.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoShine.Web.Controllers;

public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var vehicles = await _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Customer)
            .OrderBy(v => v.PlateNumber)
            .ToListAsync();

        return View(vehicles);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCustomersAsync();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("CustomerId,PlateNumber,Brand,Model,Year")]
        Vehicle vehicle)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == vehicle.CustomerId);

        if (!customerExists)
        {
            ModelState.AddModelError(
                nameof(vehicle.CustomerId),
                "Please select an existing customer.");
        }

        if (string.IsNullOrWhiteSpace(vehicle.PlateNumber))
        {
            ModelState.AddModelError(
                nameof(vehicle.PlateNumber),
                "Plate number is required.");
        }
        else
        {
            vehicle.PlateNumber = vehicle.PlateNumber.Trim();

            var plateExists = await _context.Vehicles
                .AnyAsync(v => v.PlateNumber == vehicle.PlateNumber);

            if (plateExists)
            {
                ModelState.AddModelError(
                    nameof(vehicle.PlateNumber),
                    "This plate number already exists.");
            }
        }

        if (!ModelState.IsValid)
        {
            await PopulateCustomersAsync();

            return View(vehicle);
        }

        _context.Vehicles.Add(vehicle);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCustomersAsync()
    {
        var customers = await _context.Customers
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync();

        ViewBag.Customers = new SelectList(
            customers,
            nameof(Customer.Id),
            nameof(Customer.Name));

        ViewBag.HasCustomers = customers.Count > 0;
    }
}