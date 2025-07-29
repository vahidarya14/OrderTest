using RedMaskFramework.DDD;

namespace Domain;

public class Factor:IAggregateRoot
{
    public long Id { get; set; }
    public DateTime DueDate { get; set; }
    public long Amount { get; set; }
    public long OrderId { get; set; }
    public Guid InvoiceId { get; set; }
    public FactorStatus Status { get; set; }
}

public enum FactorStatus
{
    Pending,
    Paid
}
