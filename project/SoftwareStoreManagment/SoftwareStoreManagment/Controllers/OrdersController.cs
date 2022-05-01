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
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        [Authorize]
        public async Task<IActionResult> Index(string clients)
        {
            if (String.IsNullOrEmpty(clients))
            {
                var applicationDbContext = _context.Orders.Include(o => o.Client).Include(o => o.Product);
                return View(await applicationDbContext.ToListAsync());

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
                            var searchByID = await _context.Orders.Include(o => o.Client).Include(o => o.Product).Where(c => c.OrderId == Int32.Parse(clients)).ToListAsync();

                            if (searchByID.Any())
                            {
                                return View(searchByID);
                            }
                            else return View();
                        }
                    }
                }

                var searchByName = await _context.Orders.Include(o => o.Client).Include(o => o.Product).Where(c => c.Product.ProductName.Contains(clients)).ToListAsync();
                var searchByNameClient = await _context.Orders.Include(o => o.Client).Include(o => o.Product).Where(c => c.Client.Name.Contains(clients)).ToListAsync();
              
                if (searchByName.Any())
                {
                    return View(searchByName);
                }
                else if (searchByNameClient.Any())
                {
                    return View(searchByNameClient);
                }
                else
                    return View();
            }
        }


        // GET: Orders/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,ProductId,ClientId,DateOfPurchase")] Order order)
        {
            if (ModelState.ErrorCount==2)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", order.ClientId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", order.ProductId);
            TempData["success"] = "Order created successfully";
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            var product = _context.Products.Find(order.ProductId);
            var client = _context.Clients.Find(order.ClientId);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", order.Client.Name);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", order.Product.ProductName);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("OrderId,ProductId,ClientId,DateOfPurchase")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }
            order = await _context.Orders.FindAsync(id);
            var product = _context.Products.Find(order.ProductId);
             var client = _context.Clients.Find(order.ClientId);
            if (ModelState.IsValid)  
            {
                try
                {  
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
          
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "Name", order.Client.Name);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", order.Product.ProductName);
            TempData["success"] = "Order edited successfully";
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .Include(o => o.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            //   var order = await _context.Orders.Include(o => o.Client).Include(o => o.Product).Where(c => c.OrderId == id).FirstAsync(id);
            // var order = await _context.Orders.FindAsync(id
             var order = _context.Orders.Find(id);
            var product = _context.Products.Find(order.ProductId);
            if (order != null || product!=null)
            {
                DateTime now = DateTime.Now;
                DateTime old = order.DateOfPurchase;
                old = old.AddMonths(order.Product.Warranty);
                if (old > now)
                {
                    _context.Orders.Remove(order);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Order returned successfully";
                    return RedirectToAction(nameof(Index));
                }
                else { return RedirectToAction(nameof(Index)); }
            }
            else { return RedirectToAction(nameof(Index)); }
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
