using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkOrderFlow.Web.Data;
using WorkOrderFlow.Web.Models;

namespace WorkOrderFlow.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
       public async Task<IActionResult> Index(string? search)
{
    var customers = _context.Customers.AsQueryable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        customers = customers.Where(c =>
            c.FullName.Contains(search) ||
            (c.CompanyName != null && c.CompanyName.Contains(search)) ||
            c.Phone.Contains(search) ||
            (c.Email != null && c.Email.Contains(search)) ||
            (c.Address != null && c.Address.Contains(search)));
    }

    ViewData["CurrentSearch"] = search;

    return View(await customers
        .OrderByDescending(c => c.CreatedAt)
        .ToListAsync());
}

        // GET: Customers/Details/5
      
        public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var customer = await _context.Customers
            .FirstOrDefaultAsync(m => m.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        var quotes = await _context.Quotes
            .Where(q => q.CustomerId == customer.Id)
            .OrderByDescending(q => q.CreatedAt)
            .ToListAsync();

        var workOrders = await _context.WorkOrders
            .Include(w => w.Quote)
            .Where(w => w.CustomerId == customer.Id)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

        ViewData["Quotes"] = quotes;
        ViewData["WorkOrders"] = workOrders;

        ViewData["QuoteTotal"] = quotes.Sum(q => q.LaborCost + q.PartsCost - q.Discount);

        ViewData["OpenWorkOrders"] = workOrders.Count(w =>
            w.Status != WorkOrderStatus.Completed &&
            w.Status != WorkOrderStatus.Delivered &&
            w.Status != WorkOrderStatus.Cancelled);

        return View(customer);
    }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,CompanyName,Phone,Email,Address,Notes,CreatedAt")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,CompanyName,Phone,Email,Address,Notes,CreatedAt")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
