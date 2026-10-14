using AutoShine.Web.Data;
using AutoShine.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoShine.Web.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.OrderBy(service => service.Name).ToListAsync();
            return View(services);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Name,Description,Type,CurrentPrice,EstimatedMinutes,IsActive")]
            Service service)
        {
            if (!ModelState.IsValid)
            {
                return View(service);
            }

            _context.Services.Add(service);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Type,CurrentPrice,EstimatedMinutes,IsActive")] Service submittedService)
        {
            if (id != submittedService.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(submittedService);
            }

            var service = await _context.Services.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            service.Name = submittedService.Name;
            service.Description = submittedService.Description;
            service.Type = submittedService.Type;
            service.CurrentPrice = submittedService.CurrentPrice;
            service.EstimatedMinutes = submittedService.EstimatedMinutes;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
