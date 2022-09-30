using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class SpecialTagController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialTagController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/SpecialTag
        public async Task<IActionResult> Index()
        {
            return View(await _context.SpecialTags.ToListAsync());
        }

        // GET: Admin/SpecialTag/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTags
                .FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        #region Create
        // GET: Admin/SpecialTag/Create
        public IActionResult Create()
        {
            TempData.Remove("Create");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] SpecialTag specialTag)
        {
            bool exists = await _context.SpecialTags.SingleOrDefaultAsync(s => s.Name.ToLower() == specialTag.Name.ToLower()) != null;
            if (exists)
            {
                ModelState.AddModelError(nameof(specialTag.Name), $"Special Tag \'{specialTag.Name}\' Already Exsists");
            }
            if (ModelState.IsValid)
            {
                _context.Add(specialTag);
                await _context.SaveChangesAsync();
                TempData["Create"] = "Special Tag \'" + specialTag.Name + "\' successfully added";
                return RedirectToAction(nameof(Index));
            }
            return View(specialTag);
        }
        #endregion

        #region Edit
        // GET: Admin/SpecialTag/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            TempData.Remove("Edit");
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTags.FindAsync(id);
            if (specialTag == null)
            {
                return NotFound();
            }
            TempData["OldTagName"] = specialTag.Name;
            return View(specialTag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] SpecialTag specialTag)
        {
            if (id != specialTag.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialTagExists(specialTag.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Edit"] = "Special Tage Edited From \'" + TempData["OldTagName"]
                    + "\' To \'" + specialTag.Name + "\' Successfully.";
                TempData.Remove("OldTagName");
                return RedirectToAction(nameof(Index));
            }
            return View(specialTag);
        }
        #endregion

        #region Delete
        // GET: Admin/SpecialTag/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            TempData.Remove("Delete");
            if (id == null)
            {
                return NotFound();
            }

            var specialTag = await _context.SpecialTags
                .FirstOrDefaultAsync(m => m.ID == id);
            if (specialTag == null)
            {
                return NotFound();
            }

            return View(specialTag);
        }

        // POST: Admin/SpecialTag/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialTag = await _context.SpecialTags.FindAsync(id);
            _context.SpecialTags.Remove(specialTag);
            await _context.SaveChangesAsync();
            TempData["Delete"] = "Special Tag \'" + specialTag.Name + "\' Deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        private bool SpecialTagExists(int id)
        {
            return _context.SpecialTags.Any(e => e.ID == id);
        }
    }
}
