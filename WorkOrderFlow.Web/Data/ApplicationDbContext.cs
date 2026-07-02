using Microsoft.EntityFrameworkCore;
using WorkOrderFlow.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace WorkOrderFlow.Web.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Quote> Quotes => Set<Quote>();

    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();

    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();

    public DbSet<WorkOrderMaterial> WorkOrderMaterials => Set<WorkOrderMaterial>();

    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    public DbSet<WorkOrderStatusHistory> WorkOrderStatusHistories => Set<WorkOrderStatusHistory>();

}