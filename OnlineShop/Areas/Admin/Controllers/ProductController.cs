using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _he;
        public ProductController(ApplicationDbContext context, IWebHostEnvironment he)
        {
            _context = context;
            _he = he;
        }

        #region Product Home
        // GET: Admin/Product
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.ProductState).Include(p => p.ProductType);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(int? low, int? high)
        {
            if (low == null)
            {
                decimal _ = await _context.Products.MinAsync(p => p.Price);
                low = (int?)_;
            }
            if(high == null)
            {
                decimal _ = await _context.Products.MaxAsync(p => p.Price);
                high = (int?)_;
            }
            if(low > high)
            {
                int? temp = low;
                low = high;
                high = temp;
            }
            var products = await _context.Products.Include(p => p.ProductType).Include(p => p.ProductState)
                .Where(p => p.Price >= low && p.Price <= high).ToListAsync();
            return View(products);
        }

        #endregion

        #region Details
        // GET: Admin/Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductState)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        #endregion

        #region Create
        // GET: Admin/Product/Create
        public IActionResult Create()
        {
            TempData.Remove("Create");
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "ID", "Name");
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ID", "Type");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile image)
        {
            bool exists = await _context.Products.SingleOrDefaultAsync(p => p.Name.ToLower() == product.Name.ToLower()) != null;
            if (exists)
            {
                ModelState.AddModelError(nameof(product.Name), $"Product \'{product.Name}\' Already Exsists");
            }

            if (ModelState.IsValid)
            {
                product.Image = image == null ? "Images/Image not available.jpg" : await GetImagePath(image);

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Create"] = $"\'{product.Name}\' Successfully Added";
                return RedirectToAction(nameof(Index));
            }
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "ID", "Name", product.SpecialTagId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ID", "Type", product.ProductTypeId);
            return View(product);
        }
        private async Task<string> GetImagePath(IFormFile img)
        {
            string path = Path.Combine(_he.WebRootPath + @"\Images\", Path.GetFileName(img.FileName));
            using(FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await img.CopyToAsync(fileStream);
            }

            return "Images/" + img.FileName;
        }
        #endregion

        #region Edit
        // GET: Admin/Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            TempData.Remove("Edit");
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "ID", "Name", product.SpecialTagId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ID", "Type", product.ProductTypeId);
            TempData["imgPath"] = product.Image;
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //If we changed IFormFile variable name from "image" to any other name it causes a propblem,
        //i think it because of auto matching for model parameter name (insensitive case image == Image).
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Image,ProductColor,Quantity,ProductTypeId,SpecialTagId")] Product product, IFormFile image)
        {
            if (id != product.ID)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                product.Image = image == null ? "Images/Image not available.jpg" : await GetImagePath(image);

                //now we neet to remove the old image of product
                string oldImageName = TempData["imgPath"].ToString();
                TempData.Remove("imgPath");
                RemoveImage(oldImageName);

                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Edit"] = $"Product \'{product.Name}\' Successfully Updated";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
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
            ViewData["SpecialTagId"] = new SelectList(_context.SpecialTags, "ID", "Name", product.SpecialTagId);
            ViewData["ProductTypeId"] = new SelectList(_context.ProductTypes, "ID", "Type", product.ProductTypeId);
            return View(product);
        }
        #endregion

        #region Delete
        // GET: Admin/Product/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            TempData.Remove("Delete");

            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductState)
                .Include(p => p.ProductType)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Delete"] = $"Product \'{product.Name}\' Successfully Deleted";

                //Now we also need to remove product image, but if it is available only
                RemoveImage(product.Image);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ID == id);
        }

        private void RemoveImage(string productImage)
        {
            if (productImage != "Images/Image not available.jpg")
            {
                string imageToRemove = Path.Combine(_he.WebRootPath, productImage);
                System.IO.File.Delete(imageToRemove);
            }
        }
    }
}
