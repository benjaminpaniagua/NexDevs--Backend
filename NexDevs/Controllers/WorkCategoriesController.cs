using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexDevs.Context;
using NexDevs.Models;

namespace NexDevs.Controllers
{
    public class WorkCategoriesController : Controller
    {
        private readonly DbContextNetwork _context;

        public WorkCategoriesController(DbContextNetwork context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Index(string search)
        {
            var categories = from user in _context.WorkCategories
                             select user;

            if (!String.IsNullOrEmpty(search))
            {
                categories = categories.Where(workCategorie => workCategorie.CategoryName.Contains(search));
            }

            return View(await categories.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] WorkCategorie workCategorie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workCategorie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(workCategorie);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var workCategorie = await _context.WorkCategories.FindAsync(id);
            if (workCategorie == null)
            {
                return NotFound();
            }
            return View(workCategorie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] WorkCategorie workCategorie)
        {
            if (ModelState.IsValid)
            {
                var temp = _context.WorkCategories.FirstOrDefault(x => x.CategoryId == workCategorie.CategoryId);

                if (temp != null)
                {
                    try
                    {
                        temp.CategoryId = workCategorie.CategoryId;
                        temp.CategoryName = workCategorie.CategoryName;
                        temp.CategoryImageUrl = workCategorie.CategoryImageUrl;

                        _context.WorkCategories.Update(temp);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error: " + ex.InnerException; //build a message error to show whith one alert
                        return View(workCategorie); //send the user to the view

                    }
                }
                else
                {
                    return NotFound();
                }
            }
            {
                TempData["Error"] = "Verify data"; //build a message error to show whith one alert

                return View(workCategorie);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.WorkCategories.FirstOrDefaultAsync(u => u.CategoryId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workCategorie = await _context.WorkCategories.FindAsync(id);

            if (workCategorie == null)
            {
                return View();
            }
            _context.WorkCategories.Remove(workCategorie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workCategorie = await _context.WorkCategories.FirstOrDefaultAsync(u => u.CategoryId == id);

            if (workCategorie == null)
            {
                return NotFound();
            }

            return View(workCategorie);
        }
    }
}
