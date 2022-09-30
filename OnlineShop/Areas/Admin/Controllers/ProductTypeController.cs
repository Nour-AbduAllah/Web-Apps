using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductTypeController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProductTypeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View(_db.ProductTypes.ToList());
        }

        #region Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes obj)
        {
            bool exists = await _db.ProductTypes.SingleOrDefaultAsync(p => p.Type.ToLower() == obj.Type.ToLower()) != null;
            if (exists)
            {
                ModelState.AddModelError(nameof(obj.Type), $"Product Type \'{obj.Type}\' Already Exsists");
            }

            if (ModelState.IsValid)
            {
                _db.ProductTypes.Add(obj);
                await _db.SaveChangesAsync();
                TempData["Create"] = "Product type \'"+obj.Type+ "\' successfully added";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        #endregion

        #region Edit
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ProductTypes pt = _db.ProductTypes.Find(id);
            if(pt == null)
            {
                return NotFound();
            }
           
            return View(pt);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< IActionResult> Edit(ProductTypes obj)
        {
            bool exists = await _db.ProductTypes.SingleOrDefaultAsync(p => p.Type.ToLower() == obj.Type.ToLower()) != null;
            if (exists)
            {
                ModelState.AddModelError(nameof(obj.Type), $"Product Tag \'{obj.Type}\' Already Exsists");
            }

            if (ModelState.IsValid)
            {
                _db.ProductTypes.Update(obj);
                await _db.SaveChangesAsync();
                TempData["Edit"] = "Product type changed to \'" + obj.Type + "\' successfuly";
                return RedirectToAction("Index");
            }
            return View(obj);
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            { return NotFound(); }
            ProductTypes pt = await _db.ProductTypes.FindAsync(id);
            if (pt == null)
            { return NotFound(); }
            return View(pt);
        }
        #endregion

        #region Delete
        public IActionResult Delete(int? id)
        {
            if (id == null)
            { return NotFound(); }

            ProductTypes pt = _db.ProductTypes.Find(id);
            if (pt == null)
            { return NotFound(); }
            return View(pt);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete(ProductTypes pt)
        {
            _db.ProductTypes.Remove(pt);
            await _db.SaveChangesAsync();
            TempData["Delete"] = "Product type \'"+ pt.Type + "\' successfully deleted";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
