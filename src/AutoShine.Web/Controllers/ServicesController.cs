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

    }
}
