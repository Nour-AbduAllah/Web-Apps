using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserController : Controller
    {
        readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public object DataTime { get; private set; }

        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> Index(/*int? page*/)
        {
            //page ??= 1;
            //int pageSize = 3;
            //List<ApplicationUser> products = await _context.ApplicationUsers.ToListAsync();

            //return View(await products.ToPagedListAsync((int)page, pageSize));
            return View(await _context.ApplicationUsers.ToListAsync());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _context.ApplicationUsers.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.UserName = user.Email;
                //Confirm user Email to allow Login operation
                user.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(user, user.PasswordHash);
                if (result.Succeeded)
                {
                    TempData["Create"] = "User has been signed-up successfully";
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            ApplicationUser user = await _context.ApplicationUsers.FindAsync(id);
            if(user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            if(id != user.Id)
            {
                return NotFound();
            }
            var _user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if(_user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //Why did we do this?
                _user.FirstName = user.FirstName;
                _user.LastName = user.LastName;
                var result = await _userManager.UpdateAsync(_user);
                if (result.Succeeded)
                {
                    TempData["Edit"] = "User Updated Successfully";
                    return RedirectToAction(nameof(Index));
                }
                foreach (var err in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var user = await _context.ApplicationUsers.SingleOrDefaultAsync(user => user.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user =  await _context.ApplicationUsers.SingleOrDefaultAsync(u => u.Id == id);
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LockOut(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> LockOut(ApplicationUser user)
        {
            var _user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == user.Id);
            if(_user == null)
            {
                return NotFound();
            }
            _user.LockoutEnd = DateTime.Now.AddYears(100);
            int rowAffected = await _context.SaveChangesAsync();
            if(rowAffected > 0)
            {
                TempData["Save"] = "User Account Locked out Successfuly";
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View(user);
        }
        public async Task<IActionResult> Activate(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Activate(ApplicationUser user)
        {
            var _user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == user.Id);
            if (_user == null)
            {
                return NotFound();
            }
            _user.LockoutEnd = null;
            int rowAffected = await _context.SaveChangesAsync();
            if (rowAffected > 0)
            {
                TempData["Save"] = "User Account Activated Successfuly";
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            return View(user);
        }
    }
}
