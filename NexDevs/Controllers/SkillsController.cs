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

        public async Task<IActionResult> Index(string search)
        {

            var skills = from skill in _context.Skills
                         select skill;

            if (!String.IsNullOrEmpty(search))
            {
                skills = skills.Where(skill => skill.SkillName.Contains(search));
            }

            return View(await skills.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind] Skill skill)
        {
            if (ModelState.IsValid)
            {
                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(skill);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }
            return View(skill);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] Skill skill)
        {
            if (ModelState.IsValid)
            {
                var temp = _context.Skills.FirstOrDefault(x => x.Id == skill.Id);
                if (temp != null)
                {
                    try
                    {
                        temp.SkillName = skill.SkillName;

                        _context.Skills.Update(temp);
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error: " + ex.InnerException?.Message;
                        return View(skill);
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return View(skill);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var skill = await _context.Skills.FirstOrDefaultAsync(u => u.Id == id);

            if (skill == null)
            {
                return NotFound();
            }

            return View(skill);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var skill = await _context.Skills.FirstOrDefaultAsync(x => x.Id == id);
            if (skill == null) 
            {
                return NotFound();
            }

            return View(skill);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return View();
            }
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
