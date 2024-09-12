using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexDevs.Context;
using NexDevs.Controllers;
using NexDevs.Models;

namespace NexDevs.Controllers
{
    public class SkillsController : Controller
    {

        private readonly DbContextNetwork _context;

        public SkillsController(DbContextNetwork context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var listado = await _context.Skills.ToListAsync();
            return View(listado);
        }
    }
}
