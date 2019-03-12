using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBooking.Data;
using GymBooking.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Routing;

namespace GymBooking.Controllers
{
    [Authorize/*(Roles = "Admin")*/]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymClassesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.GymClass.ToListAsync());
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gymClass);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GymClassExists(gymClass.Id))
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
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClass.FindAsync(id);
            _context.GymClass.Remove(gymClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return _context.GymClass.Any(e => e.Id == id);
        }


        public enum BookMode
        {
            Book,
            Unbook,
            Show
        }

        // GET: GymClasses/Book/5
        public async Task<IActionResult> Book(int? id, string book, string unbook)
        {
            var model = new BookViewModel();

            var bookingMode = string.IsNullOrEmpty(book) ? false : true;

            if (id == null)
            {
                return NotFound();
            }

            var bookMode = BookMode.Show;
            if(!string.IsNullOrEmpty(book))
            {
                bookMode = BookMode.Book;
            }
            else if(!string.IsNullOrEmpty(unbook))
            {
                bookMode = BookMode.Unbook;
            }

            var gymClass = await _context.GymClass
                .Where(m => m.Id == id)
                .Include(a => a.AttendingMembers).FirstOrDefaultAsync();

            if (gymClass == null)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                model.Message = "User Not Logged in";
                return View(gymClass);
            }

            ApplicationUser currentUser = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
            if (currentUser == null)
            {
                model.Message = "User Not Found";
                return View(gymClass);
            }

            var booked = _context.ApplicationUserGymClass.
                FirstOrDefault(a => a.GymClassId == gymClass.Id && a.ApplicationUserId == currentUser.Id);

            model.IsBooked = false;
            if (booked != null)
            {
                model.IsBooked = true;
                //model.Message = "Allready Booked";
            }

            if (bookMode == BookMode.Book)
            {
                if (booked != null)
                {
                    model.Message = "Allready Booked";
                }
                else
                {
                    _context.ApplicationUserGymClass.Add(new ApplicationUserGymClass() { GymClassId = gymClass.Id, ApplicationUserId = currentUser.Id });
                    _context.SaveChanges();
                    model.Message = "Class has been Booked";
                    model.IsBooked = true;
                }
            }
            else if(bookMode == BookMode.Unbook)
            {
                if (booked != null)
                {
                    _context.ApplicationUserGymClass.Remove(booked);
                    _context.SaveChanges();
                    model.IsBooked = false;
                    model.Message = "Class has been Unbooked";
                }
                else
                {
                    model.Message = "Not Booked";
                }
            }

            model.Description = gymClass.Description;
            model.Duration = gymClass.Duration;
            model.Id = gymClass.Id;
            model.Name = gymClass.Name;
            model.StartTime = gymClass.StartTime;
            return View(model);
        }

        // GET: GymClasses/Book/5
        //public async Task<IActionResult> Unbook(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var gymClass = await _context.GymClass
        //        .Where(m => m.Id == id)
        //        .Include(a => a.AttendedClasses).FirstOrDefaultAsync();

        //    if (gymClass == null)
        //    {
        //        return NotFound();
        //    }

        //    if (!User.Identity.IsAuthenticated)
        //    {
        //        ViewBag.Msg = "User Not Logged in";
        //        return View(gymClass);
        //    }

        //    ApplicationUser currentUser = _context.Users.FirstOrDefault(a => a.UserName == User.Identity.Name);
        //    if (currentUser == null)
        //    {
        //        ViewBag.Msg = "User Not Found";
        //        return View(gymClass);
        //    }

        //    var booked = _context.ApplicationUserGymClass.
        //        FirstOrDefault(a => a.GymClassId == gymClass.Id && a.ApplicationUserId == currentUser.Id);
        //    if (booked != null)
        //    {
        //        _context.ApplicationUserGymClass.Remove(booked);
        //        _context.SaveChanges();
        //        ViewBag.Msg = "Class has been Unbooked";
        //    }
        //    else
        //    {
        //        ViewBag.Msg = "Not Booked";
        //    }

        //    return RedirectToAction(nameof(Book), new RouteValueDictionary(new { controller = "GymClasses", action = "Book", Id = id }));
        //}

        
    }
}
