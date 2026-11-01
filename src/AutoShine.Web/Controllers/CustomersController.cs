using AutoShine.Web.Models;
using AutoShine.Web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoShine.Web.Controllers;

public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _context.Customers.ToListAsync();
        return View(customers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (!ModelState.IsValid)
        {
            return View(customer);
        }

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id ==null)
        {
            return NotFound();
        }
        var customer = await _context.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit (int id, [Bind("Id,Name,PhoneNumber,Email")] Customer submittedCustomer)
    {
        if (id != submittedCustomer.Id)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(submittedCustomer.Name))
        {
            ModelState.AddModelError(nameof(Customer.Name), "Name is required.");
        }

        if (string.IsNullOrWhiteSpace(submittedCustomer.PhoneNumber))
        {
            ModelState.AddModelError(nameof(Customer.PhoneNumber), "Phone number is required.");
        }

        if (!ModelState.IsValid)
        {
            return View(submittedCustomer);
        }

        var customer =  await _context.Customers.FindAsync(id);

        if (customer == null)
        {
            return NotFound();
        }

        customer.Name = submittedCustomer.Name.Trim();
        customer.PhoneNumber = submittedCustomer.PhoneNumber.Trim();
        customer.Email = string.IsNullOrWhiteSpace(submittedCustomer.Email) ? null : submittedCustomer.Email.Trim();

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
