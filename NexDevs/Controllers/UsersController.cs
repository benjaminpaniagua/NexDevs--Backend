using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexDevs.Context;
using NexDevs.Models;

namespace NexDevs.Controllers
{
    public class UsersController : Controller
    {

        private readonly DbContextNetwork _context;

        public UsersController(DbContextNetwork context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View(_context.Users.ToList());
        }

        [HttpGet]
        //action encharge to show the form to create a new book
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind]User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind] User user)
        {
            //validations
            if (ModelState.IsValid)
            {
                var temp = _context.Users.FirstOrDefault(x => x.UserId == user.UserId);

                if (temp != null)
                {
                    try
                    {
                        temp.FirstName = user.FirstName;
                        temp.LastName = user.LastName;
                        temp.Email = user.Email;
                        temp.Password = user.Password;
                        temp.Province = user.Province;
                        temp.City = user.City;
                        temp.Bio = user.Bio;
                        temp.ProfilePictureUrl = user.ProfilePictureUrl;
                        temp.ProfileType = user.ProfileType;

                        _context.Users.Update(temp);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = "Error: " + ex.InnerException; //build a message error to show whith one alert
                        return View(user); //send the user to the view

                    }
                }
                else
                {
                    return NotFound();
                }
            }
            {
                TempData["Error"] = "Verify data"; //build a message error to show whith one alert

                return View(user);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

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
            var user = await _context.Users.FindAsync(id);

            if(user == null)
            {
                return View();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
