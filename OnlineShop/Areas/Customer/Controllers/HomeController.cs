using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShop.Models;
using OnlineShop.Data;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Utility;
using X.PagedList;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Index
        public async Task<IActionResult> Index( int? page)
        {
            var pageNumber = page ?? 1;
            int pageSize = 3;
            List<Product> products = await _context.Products.Include(p => p.ProductState).Include(p => p.ProductType).ToListAsync();
            
            return View(await products.ToPagedListAsync(pageNumber, pageSize));
        }
        #endregion

        #region Details
        public async Task<IActionResult> Details(int? id)
        {
            if(id == null)
            { 
                return NotFound();
            }
            Product product = await _context.Products
                                            .Include(p=>p.ProductState)
                                            .Include(p=>p.ProductType)
                                            .FirstOrDefaultAsync(p=>p.ID == id);
            if(product == null)
            {
                return NotFound();
            }
            ViewBag.returnUrl = Request.Headers["Referer"].ToString();
            return View(product);
        }

        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product product = await _context.Products
                                                    .Include(p => p.ProductState)
                                                    .Include(p => p.ProductType)
                                                    .FirstOrDefaultAsync(p => p.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if(products == null)
            {
                products = new List<Product>();
            }
            products.Add(product);
            HttpContext.Session.Set("products", products);
            return View(product);
        }

        #endregion

        #region Cart Control

        public IActionResult Cart(string returnUrl)
        {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products == null || products.Count == 0)
            {
                return View(nameof(EmptyCart));
            }
            return View(products);
        }

        public IActionResult EmptyCart()
        {
            return View();
        }

        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product product = HttpContext.Session.Get<List<Product>>("products")
                                                 .FirstOrDefault(p => p.ID == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ActionName("Remove")]
        public IActionResult ConfirmedRemove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products != null)
            {
                Product product = products.FirstOrDefault(p => p.ID == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set<List<Product>>("products", products);
                }
            }
            return RedirectToAction(nameof(Cart));
        }

        #endregion

        #region Privacy

        public IActionResult Privacy()
        {
            return View();
        }

        #endregion

        #region x

        #endregion

        #region x

        #endregion

        #region x

        #endregion
    }
}
