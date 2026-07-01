using Microsoft.EntityFrameworkCore;
using WorkOrderFlow.Web.Data;
using WorkOrderFlow.Web.Models;

namespace WorkOrderFlow.Web.Services;

public class InventoryStockService
{
    private readonly ApplicationDbContext _context;

    public InventoryStockService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ApplyStockMovementAsync(
        int inventoryItemId,
        int quantityChange,
        InventoryTransactionType transactionType,
        string? notes,
        int? workOrderId = null)
    {
        var inventoryItem = await _context.InventoryItems
            .FirstOrDefaultAsync(i => i.Id == inventoryItemId);

        if (inventoryItem == null)
        {
            return false;
        }

        var newQuantity = inventoryItem.QuantityOnHand + quantityChange;

        if (newQuantity < 0)
        {
            return false;
        }

        inventoryItem.QuantityOnHand = newQuantity;

        _context.InventoryTransactions.Add(new InventoryTransaction
        {
            InventoryItemId = inventoryItem.Id,
            WorkOrderId = workOrderId,
            Type = transactionType,
            QuantityChange = quantityChange,
            QuantityAfter = inventoryItem.QuantityOnHand,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        return true;
    }
}