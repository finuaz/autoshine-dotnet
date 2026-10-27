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

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        return View(vehicle);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit ( 
        int id,
        [Bind("Id,PlateNumber,Brand,Model,Year")]
        Vehicle submittedVehicle
        )
    {
        if (id != submittedVehicle.Id)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(submittedVehicle.PlateNumber))
        {
            ModelState.AddModelError(
                nameof(submittedVehicle.PlateNumber),
                "Plate number is required.");
        }

        if (string.IsNullOrWhiteSpace(submittedVehicle.Brand))
        {
            ModelState.AddModelError(
                nameof(submittedVehicle.Brand),
                "Brand is required.");
        }


        if (string.IsNullOrWhiteSpace(submittedVehicle.Model))
        {
            ModelState.AddModelError(
                nameof(submittedVehicle.Model),
                "Model is required.");
        }

        if (submittedVehicle.Year is <= 0)
        {
            ModelState.AddModelError(
                nameof(submittedVehicle.Year),
                "Year must be greater than zero.");
        }

        if (!ModelState.IsValid)
        {
            return View(submittedVehicle);
        }

        submittedVehicle.PlateNumber = submittedVehicle.PlateNumber.Trim();

        var vehicle = await _context.Vehicles.FindAsync(id);

        if (vehicle == null)
        {
            return NotFound();
        }

        var plateExists = await _context.Vehicles.AnyAsync(v =>
            v.Id != id && v.PlateNumber == submittedVehicle.PlateNumber);   

        if (plateExists)
        {
            ModelState.AddModelError(nameof(submittedVehicle.PlateNumber), "This plate number already exists.");

            return View(submittedVehicle);
        }

        vehicle.PlateNumber = submittedVehicle.PlateNumber;
        vehicle.Brand = submittedVehicle.Brand;
        vehicle.Model = submittedVehicle.Model;
        vehicle.Year = submittedVehicle.Year;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}