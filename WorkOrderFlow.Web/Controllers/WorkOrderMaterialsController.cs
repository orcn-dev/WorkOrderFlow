using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkOrderFlow.Web.Data;
using WorkOrderFlow.Web.Models;
using WorkOrderFlow.Web.Services;

namespace WorkOrderFlow.Web.Controllers;

public class WorkOrderMaterialsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly InventoryStockService _stockService;

    public WorkOrderMaterialsController(
        ApplicationDbContext context,
        InventoryStockService stockService)
    {
        _context = context;
        _stockService = stockService;
    }

    public async Task<IActionResult> Index()
    {
        var materials = _context.WorkOrderMaterials
            .Include(w => w.InventoryItem)
            .Include(w => w.WorkOrder)
            .OrderByDescending(w => w.UsedAt);

        return View(await materials.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var workOrderMaterial = await _context.WorkOrderMaterials
            .Include(w => w.InventoryItem)
            .Include(w => w.WorkOrder)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (workOrderMaterial == null)
        {
            return NotFound();
        }

        return View(workOrderMaterial);
    }

    public async Task<IActionResult> Create(int? workOrderId)
    {
        var material = new WorkOrderMaterial
        {
            WorkOrderId = workOrderId ?? 0,
            UsedAt = DateTime.UtcNow
        };

        await PopulateDropDownsAsync(material.WorkOrderId);

        return View(material);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(WorkOrderMaterial workOrderMaterial)
    {
        if (workOrderMaterial.QuantityUsed <= 0)
        {
            ModelState.AddModelError(nameof(workOrderMaterial.QuantityUsed), "Quantity used must be greater than zero.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropDownsAsync(workOrderMaterial.WorkOrderId, workOrderMaterial.InventoryItemId);
            return View(workOrderMaterial);
        }

        if (workOrderMaterial.UsedAt == default)
        {
            workOrderMaterial.UsedAt = DateTime.UtcNow;
        }

        _context.WorkOrderMaterials.Add(workOrderMaterial);

        var success = await _stockService.ApplyStockMovementAsync(
            workOrderMaterial.InventoryItemId,
            -workOrderMaterial.QuantityUsed,
            InventoryTransactionType.WorkOrderUsage,
            $"Material used on work order #{workOrderMaterial.WorkOrderId}",
            workOrderMaterial.WorkOrderId);

        if (!success)
        {
            ModelState.AddModelError(nameof(workOrderMaterial.QuantityUsed), "Not enough stock for this material usage.");

            await PopulateDropDownsAsync(workOrderMaterial.WorkOrderId, workOrderMaterial.InventoryItemId);
            return View(workOrderMaterial);
        }

        return RedirectToAction("Details", "WorkOrders", new { id = workOrderMaterial.WorkOrderId });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var workOrderMaterial = await _context.WorkOrderMaterials.FindAsync(id);

        if (workOrderMaterial == null)
        {
            return NotFound();
        }

        await PopulateDropDownsAsync(workOrderMaterial.WorkOrderId, workOrderMaterial.InventoryItemId);

        return View(workOrderMaterial);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, WorkOrderMaterial workOrderMaterial)
    {
        if (id != workOrderMaterial.Id)
        {
            return NotFound();
        }

        if (workOrderMaterial.QuantityUsed <= 0)
        {
            ModelState.AddModelError(nameof(workOrderMaterial.QuantityUsed), "Quantity used must be greater than zero.");
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropDownsAsync(workOrderMaterial.WorkOrderId, workOrderMaterial.InventoryItemId);
            return View(workOrderMaterial);
        }

        var existingMaterial = await _context.WorkOrderMaterials
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);

        if (existingMaterial == null)
        {
            return NotFound();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();

        if (existingMaterial.InventoryItemId == workOrderMaterial.InventoryItemId)
        {
            var quantityChange = existingMaterial.QuantityUsed - workOrderMaterial.QuantityUsed;

            if (quantityChange != 0)
            {
                var success = await _stockService.ApplyStockMovementAsync(
                    workOrderMaterial.InventoryItemId,
                    quantityChange,
                    InventoryTransactionType.WorkOrderUsageCorrection,
                    $"Material usage corrected on work order #{workOrderMaterial.WorkOrderId}",
                    workOrderMaterial.WorkOrderId);

                if (!success)
                {
                    ModelState.AddModelError(nameof(workOrderMaterial.QuantityUsed), "Stock correction could not be applied. Quantity cannot go below zero.");

                    await PopulateDropDownsAsync(workOrderMaterial.WorkOrderId, workOrderMaterial.InventoryItemId);
                    return View(workOrderMaterial);
                }
            }
        }
        else
        {
            var reverseSuccess = await _stockService.ApplyStockMovementAsync(
                existingMaterial.InventoryItemId,
                existingMaterial.QuantityUsed,
                InventoryTransactionType.WorkOrderUsageReversal,
                $"Material usage reversed because item changed on work order #{existingMaterial.WorkOrderId}",
                existingMaterial.WorkOrderId);

            if (!reverseSuccess)
            {
                ModelState.AddModelError(string.Empty, "Previous material usage could not be reversed.");

                await PopulateDropDownsAsync(workOrderMaterial.WorkOrderId, workOrderMaterial.InventoryItemId);
                return View(workOrderMaterial);
            }

            var applySuccess = await _stockService.ApplyStockMovementAsync(
                workOrderMaterial.InventoryItemId,
                -workOrderMaterial.QuantityUsed,
                InventoryTransactionType.WorkOrderUsageCorrection,
                $"Material usage corrected with a different item on work order #{workOrderMaterial.WorkOrderId}",
                workOrderMaterial.WorkOrderId);

            if (!applySuccess)
            {
                ModelState.AddModelError(nameof(workOrderMaterial.QuantityUsed), "Not enough stock for the selected replacement item.");

                await PopulateDropDownsAsync(workOrderMaterial.WorkOrderId, workOrderMaterial.InventoryItemId);
                return View(workOrderMaterial);
            }
        }

        try
        {
            _context.Update(workOrderMaterial);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!WorkOrderMaterialExists(workOrderMaterial.Id))
            {
                return NotFound();
            }

            throw;
        }

        return RedirectToAction("Details", "WorkOrders", new { id = workOrderMaterial.WorkOrderId });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var workOrderMaterial = await _context.WorkOrderMaterials
            .Include(w => w.InventoryItem)
            .Include(w => w.WorkOrder)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (workOrderMaterial == null)
        {
            return NotFound();
        }

        return View(workOrderMaterial);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var workOrderMaterial = await _context.WorkOrderMaterials
            .FirstOrDefaultAsync(m => m.Id == id);

        if (workOrderMaterial == null)
        {
            return NotFound();
        }

        var workOrderId = workOrderMaterial.WorkOrderId;

        _context.WorkOrderMaterials.Remove(workOrderMaterial);

        var success = await _stockService.ApplyStockMovementAsync(
            workOrderMaterial.InventoryItemId,
            workOrderMaterial.QuantityUsed,
            InventoryTransactionType.WorkOrderUsageReversal,
            $"Material usage deleted from work order #{workOrderId}",
            workOrderId);

        if (!success)
        {
            return BadRequest("Material usage could not be reversed.");
        }

        return RedirectToAction("Details", "WorkOrders", new { id = workOrderId });
    }

    private async Task PopulateDropDownsAsync(int? selectedWorkOrderId = null, int? selectedInventoryItemId = null)
    {
        var workOrders = await _context.WorkOrders
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync();

        var inventoryItems = await _context.InventoryItems
            .OrderBy(i => i.Name)
            .ToListAsync();

        ViewData["WorkOrderId"] = new SelectList(workOrders, "Id", "Title", selectedWorkOrderId);
        ViewData["InventoryItemId"] = new SelectList(inventoryItems, "Id", "Name", selectedInventoryItemId);
    }

    private bool WorkOrderMaterialExists(int id)
    {
        return _context.WorkOrderMaterials.Any(e => e.Id == id);
    }
}