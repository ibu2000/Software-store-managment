#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareStoreManagment.Data;
using SoftwareStoreManagment.Models;

namespace SoftwareStoreManagment.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index(string clients)
        {

            if (String.IsNullOrEmpty(clients))
            {
                var applicationDbContext2 = _context.Products;
                return View(await applicationDbContext2.ToListAsync());

            }
            else
            {
                string str = @"-?\d+(?:\.\d+)?";
                var regex = new Regex(str, RegexOptions.IgnoreCase);
                bool IsValidID = regex.IsMatch(clients);
                if (IsValidID)
                {
                    if (clients.Length < 10)
                    {
                        int number;
                        if (int.TryParse(clients, out number))
                        {
                            var searchByID = await _context.Products.Where(c => c.ProductId == Int32.Parse(clients)).ToListAsync();
                            var searchByWarranty = await _context.Products.Where(c => c.Warranty == Int32.Parse(clients)).ToListAsync();
                            if (searchByID.Any())
                            {
                                return View(searchByID);
                            }
                            else if (searchByWarranty.Any())
                            {
                                return View(searchByWarranty);
                            }
                            else return View();
                        }


                        var searchByPrice = await _context.Products.Where(c => c.Price == Double.Parse(clients)).ToListAsync();

                        if (searchByPrice.Any())
                        {
                            return View(searchByPrice);
                        }
                        else return View();
                    }
                    else return View();
                }

                var searchByName = await _context.Products.Where(c => c.ProductName.Contains(clients)).ToListAsync();
                var searchByDesc = await _context.Products.Where(c => c.Description.Contains(clients)).ToListAsync();
               
                if (searchByName.Any())
                {
                    return View(searchByName);
                }
                else if (searchByDesc.Any())
                {
                    return View(searchByDesc);
                }
                else
                    return View();
            }
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Price,Description,Warranty")] Product product)
        {
            string war = @"[0-9]+";
            var regexWarranty = new Regex(war, RegexOptions.IgnoreCase);
            bool isValidWarranty = regexWarranty.IsMatch(product.Warranty.ToString());
            if (!isValidWarranty)
            {

                ModelState.AddModelError("Warranty", "Not a valid warranty period");
            }
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["success"] = "Product created successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(long id, [Bind("ProductId,ProductName,Price,Description,Warranty")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            TempData["success"] = "Product edited successfully";
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(long id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
