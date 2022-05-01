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
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        [Authorize]
        public async Task<IActionResult> Index(string clients)
        {
            if (String.IsNullOrEmpty(clients))
            {
                var applicationDbContext2 = _context.Clients;
                return View(await applicationDbContext2.ToListAsync());

            }
            else
            {   
                string str = @"^\d+$";
                var regex = new Regex(str, RegexOptions.IgnoreCase);
                bool IsValidID = regex.IsMatch(clients);
                if (IsValidID)
                {
                    if (clients.Length < 10)
                    {
                        var searchByID = await _context.Clients.Where(c => c.ClientId == Int32.Parse(clients)).ToListAsync();
                        var searchByPhone = await _context.Clients.Where(c => c.Phone.Contains(clients)).ToListAsync();
                        if (searchByID.Any())
                        {
                            return View(searchByID);
                        }
                        else if (searchByPhone.Any())
                        {
                            return View(searchByPhone);
                        }
                        else return View();
                    }
                    else return View();
                }
              
                var searchByName = await _context.Clients.Where(c => c.Name.Contains(clients)).ToListAsync();
                var searchByEmail = await _context.Clients.Where(c => c.Email.Contains(clients)).ToListAsync();

               
                if (searchByName.Any())
                {
                    return View(searchByName);
                }
                else if (searchByEmail.Any())
                {
                    return View(searchByEmail);
                }
                else
                return View();
            }
        }



        // GET: Clients/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();

        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ClientId,Name,Phone,Email")] Client client)
        {

            const string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            bool IsValidEmail = regex.IsMatch(client.Email);        
            if(!IsValidEmail)
            {
                  ModelState.AddModelError("Email", "Not a valid email");
            }
            string name = @"^[a-zA-Z]+$";
            var regexName = new Regex(name, RegexOptions.IgnoreCase);
            bool isValidName = regexName.IsMatch(client.Name);
            if (!isValidName)
            {

                ModelState.AddModelError("Name", "Not a valid name");
            }
            string phone = @"^[08]"+"[0-9]+$";
            var regexPhone = new Regex(phone, RegexOptions.IgnoreCase);
            bool isValidPhone = regexPhone.IsMatch(client.Phone);
            if (!isValidPhone || client.Phone.Length != 10)
            {

                ModelState.AddModelError("Phone", "Not a valid phone");
            }
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                TempData["success"] = "Client created successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(client);

        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("ClientId,Name,Phone,Email")] Client client)
        {
            if (id != client.ClientId)
            {
                return NotFound();
            }
            const string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|" + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)" + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            bool IsValidEmail = regex.IsMatch(client.Email);
            if (!IsValidEmail)
            {
                ModelState.AddModelError("Email", "Not a valid email");
            }
            string name = @"^[a-zA-Z]+$";
            var regexName = new Regex(name, RegexOptions.IgnoreCase);
            bool isValidName = regexName.IsMatch(client.Name);
            if (!isValidName)
            {

                ModelState.AddModelError("Name", "Not a valid name");
            }
            string phone = @"^[08]" + "[0-9]+$";
            var regexPhone = new Regex(phone, RegexOptions.IgnoreCase);
            bool isValidPhone = regexPhone.IsMatch(client.Phone);
            if (!isValidPhone || client.Phone.Length != 10)
            {

                ModelState.AddModelError("Phone", "Not a valid phone");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Client updated successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            TempData["success"] = "Client deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(long id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }
    }
}
