using System.ComponentModel.DataAnnotations.Schema;

namespace WorkOrderFlow.Web.Models;

public class Quote
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public string Title { get; set; } = string.Empty;

    public decimal LaborCost { get; set; }

    public decimal PartsCost { get; set; }

    public decimal Discount { get; set; }

    [NotMapped]
    public decimal TotalAmount => LaborCost + PartsCost - Discount;

    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ValidUntil { get; set; }

    public string? Notes { get; set; }
}

public enum QuoteStatus
{
    Draft = 0,
    Sent = 1,
    Accepted = 2,
    Rejected = 3,
    Expired = 4
}