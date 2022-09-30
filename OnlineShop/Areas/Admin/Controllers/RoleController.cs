using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Areas.Admin.Models;
using OnlineShop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class RoleController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        ApplicationDbContext _context;
        UserManager<IdentityUser> _userManager;
        public RoleController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _roleManager.Roles.ToListAsync());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            IdentityRole role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if(role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if(role == null)
            {
                return NotFound();
            }

            bool exists = await _roleManager.RoleExistsAsync(role.Name);
            if (exists)
            {
                ModelState.AddModelError("Name", "This role already exists!");
            }
            if (ModelState.IsValid)
            {
                var response = await _roleManager.CreateAsync(role);
                if (response.Succeeded)
                {
                    ViewData["save"] = "Role \'" + role.Name + "\' created successfully";
                    return RedirectToAction("Index", "Role", new { area = "Admin" });
                }
                foreach (var err in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(role);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            if (role == null)
            {
                return NotFound();
            }
            var _role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == role.Id);
            _role.Name = role.Name;
            bool exists = await _roleManager.RoleExistsAsync(role.Name);
            if (exists)
            {
                ModelState.AddModelError("Name", "This role already exists!");
            }
            if (ModelState.IsValid)
            {
                var response = await _roleManager.UpdateAsync(_role);
                if (response.Succeeded)
                {
                    ViewData["title"] = "Edit process done successfully";
                    return RedirectToAction("Index", "Role", new { area = "Admin" });
                }
                foreach (var err in response.Errors)
                {
                    ModelState.AddModelError(string.Empty, err.Description);
                }
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if(role == null)
            {
                return NotFound();
            }
            return View(role);
        }
        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmedDelete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            var response = await _roleManager.DeleteAsync(role);
            if (response.Succeeded)
            {
                ViewData["Delete"] = "Role deleted successfuly";
                return RedirectToAction("Index", "Role", new { area = "Admin" });
            }
            foreach (var err in response.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(role);
        }

        public async Task<IActionResult> UserRole()
        {
            List<UserRoleMapping> userRole = await _context.UserRoles
                .Join(
                    _context.ApplicationUsers,
                    userole => userole.UserId,
                    user => user.Id,
                    (userole, user) => new
                    {
                        RoleId = userole.RoleId,
                        UserId = userole.UserId,
                        UserName = user.UserName
                    })
                .Join(
                    _context.Roles,
                    userole => userole.RoleId,
                    role => role.Id,
                    (userole, role) => new UserRoleMapping
                    {
                        UserId = userole.UserId,
                        RoleId = role.Id,
                        UserName = userole.UserName,
                        RoleName = role.Name
                    })
                .ToListAsync();
            //var userRole = from userloe in _context.UserRoles
            //               join role in _context.Roles on userloe.RoleId equals role.Id
            //               join user in _context.ApplicationUsers on userloe.UserId equals user.Id
            //               select new UserRoleMapping()
            //               {
            //                   UserId = userloe.UserId,
            //                   RoleId = userloe.RoleId,
            //                   UserName = user.UserName,
            //                   RoleName = role.Name
            //               };
            
            return View(userRole);
        }
        public async Task<IActionResult> Assign()
        {
            ViewData["UserId"] = new SelectList(await _context.ApplicationUsers
                .Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
                .ToListAsync(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Assign(UserRoleVm userRole)
        {
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userRole.UserId);
            bool RoleAssigned = await _userManager.IsInRoleAsync(user, userRole.RoleId);
            if (RoleAssigned)
            {
                ViewData["UserId"] = new SelectList(await _context.ApplicationUsers
                .Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
                .ToListAsync(), "Id", "UserName");

                ViewData["RoleId"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");

                ViewBag.message = "This user already has his role. If you want to edit it please go to edit page";

                return View(userRole);
            }
            var response = await _userManager.AddToRoleAsync(user, userRole.RoleId);
            if (response.Succeeded)
            {
                TempData["Create"] = "User Role Added";
                return RedirectToAction("UserRole", "Role", new { area = "Admin" });
            }
            foreach (var err in response.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            ViewData["UserId"] = new SelectList(await _context.ApplicationUsers
                .Where(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.Now)
                .ToListAsync(), "Id", "UserName");
            ViewData["RoleId"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
            return View(userRole);
        }

        public async Task<IActionResult> DeleteUserRole(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var userole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
            if (userole == null)
            {
                return NotFound();
            }

            //We need to send an object to the view with user id&Email
            //So we get current user Email/UserName from ApplicationUsers by current id
            //Then send our user object {Id, Name}
            ViewBag.UserRole = await GetUserRoleMapping(userole, id);
            return View(new UserRoleVm() { UserId = userole.UserId, RoleId = userole.RoleId});
        }
        [HttpPost]
        [ActionName("DeleteUserRole")]
        public async Task<IActionResult> DeleteUserRoleConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            IdentityUserRole<string> userRole = await _context.UserRoles.FirstOrDefaultAsync(ur => ur.UserId == id);
            if (userRole == null)
            {
                return NotFound();
            }
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userRole.UserId);
            string[] roles = { userRole.RoleId };
            UserRoleMapping urm = await GetUserRoleMapping(userRole, id);
            ViewBag.UserRole = urm;
            var response = await _userManager.RemoveFromRoleAsync(user, urm.RoleName);
            if (response.Succeeded)
            {
                TempData["Delete"] = "User Role Deleted Successfully";
                return RedirectToAction("UserRole", "Role", new { area = "Admin" });
            }
            foreach (var err in response.Errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(new UserRoleVm() { UserId = userRole.UserId, RoleId = userRole.RoleId });
        }

        private async Task< UserRoleMapping> GetUserRoleMapping(IdentityUserRole<string> userRole, string id)
        {
            //We need to send an object to the view with user id&Email
            //So we get current user Email/UserName from ApplicationUsers by current id
            //Then send our user object {Id, Name}
            string userName = await _context.ApplicationUsers.Where(u => u.Id == id)
                                                         .Select(u => u.UserName)
                                                         .FirstOrDefaultAsync();
            string roleName = await _context.Roles.Where(u => u.Id == userRole.RoleId)
                                                         .Select(u => u.Name)
                                                         .FirstOrDefaultAsync();
            return new UserRoleMapping
            {
                UserId = userRole.UserId,
                RoleId = userRole.RoleId,
                UserName = userName,
                RoleName = roleName
            };
        }
    }
}
