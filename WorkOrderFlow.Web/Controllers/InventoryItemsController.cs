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
    public class InventoryItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: InventoryItems
        public async Task<IActionResult> Index(string? search, bool lowStockOnly = false)
        {
            var inventoryItems = _context.InventoryItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                inventoryItems = inventoryItems.Where(i =>
                    i.Name.Contains(search) ||
                    (i.Sku != null && i.Sku.Contains(search)) ||
                    i.Category.Contains(search) ||
                    (i.SupplierName != null && i.SupplierName.Contains(search)) ||
                    (i.Location != null && i.Location.Contains(search)));
            }

            if (lowStockOnly)
            {
                inventoryItems = inventoryItems.Where(i => i.QuantityOnHand <= i.ReorderLevel);
            }

            ViewData["CurrentSearch"] = search;
            ViewData["LowStockOnly"] = lowStockOnly;

            return View(await inventoryItems
                .OrderBy(i => i.QuantityOnHand <= i.ReorderLevel ? 0 : 1)
                .ThenBy(i => i.Name)
                .ToListAsync());
        }

        // GET: InventoryItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }

        // GET: InventoryItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InventoryItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Sku,Category,QuantityOnHand,ReorderLevel,UnitCost,SalePrice,SupplierName,Location,Notes,CreatedAt")] InventoryItem inventoryItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inventoryItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(inventoryItem);
        }

        // GET: InventoryItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return NotFound();
            }
            return View(inventoryItem);
        }

        // POST: InventoryItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Sku,Category,QuantityOnHand,ReorderLevel,UnitCost,SalePrice,SupplierName,Location,Notes,CreatedAt")] InventoryItem inventoryItem)
        {
            if (id != inventoryItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inventoryItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryItemExists(inventoryItem.Id))
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
            return View(inventoryItem);
        }

        // GET: InventoryItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inventoryItem == null)
            {
                return NotFound();
            }

            return View(inventoryItem);
        }

        // POST: InventoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inventoryItem = await _context.InventoryItems.FindAsync(id);
            if (inventoryItem != null)
            {
                _context.InventoryItems.Remove(inventoryItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InventoryItemExists(int id)
        {
            return _context.InventoryItems.Any(e => e.Id == id);
        }
    }
}
