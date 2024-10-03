using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NexDevs.Context;
using NexDevs.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NexDevs.Controllers
{
    public class WorkProfilesController : Controller
    {
        private readonly DbContextNetwork _context;

        public WorkProfilesController(DbContextNetwork context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index(string search)
        {
            var workProfiles = from workProfile in _context.WorkProfiles select workProfile;

            if (!string.IsNullOrEmpty(search))
            {
                workProfiles = workProfiles.Where(workProfile => workProfile.Name.Contains(search));
            }

            return View(await workProfiles.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var workProfile = await _context.WorkProfiles
                .FirstOrDefaultAsync(m => m.WorkId == id);

            if (workProfile == null)
            {
                return NotFound();
            }

            return View(workProfile);
        }


        public IActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(_context.WorkCategories, "CategoryId", "CategoryName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkId,Name,Email,Number,Password,Province,City,WorkDescription,ProfilePictureUrl,CategoryId,ProfileType")] WorkProfile workProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workProfile);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workProfile = await _context.WorkProfiles.FindAsync(id);
            if (workProfile == null)
            {
                return NotFound();
            }

            return View(workProfile);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkId,Name,Email,Number,Password,Province,City,WorkDescription,ProfilePictureUrl,CategoryId,ProfileType")] WorkProfile workProfile)
        {
            if (id != workProfile.WorkId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkProfileExists(workProfile.WorkId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(workProfile);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workProfile = await _context.WorkProfiles
                .FirstOrDefaultAsync(m => m.WorkId == id);
            if (workProfile == null)
            {
                return NotFound();
            }

            return View(workProfile);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workProfile = await _context.WorkProfiles.FindAsync(id);
            _context.WorkProfiles.Remove(workProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool WorkProfileExists(int id)
        {
            return _context.WorkProfiles.Any(e => e.WorkId == id);
        }
    }
}
