using RedMaskFramework.DDD;

namespace Domain;

public class Factor:IAggregateRoot
{
    public long Id { get; set; }
    public DateTime DueDate { get; set; }
    public long Amount { get; set; }
    public long OrderId { get; set; }
    public Order Order { get; set; }
    public Guid InvoiceId { get; set; }
    public FactorStatus Status { get; set; }
}

public enum FactorStatus
{
    Pending,
    Paid
}
//.1 مشتری )حساب(: نمایانگر یک مشتری با فیلدها
//ی مانند ،LastName ،FirstName ،CustomerID//.Role و WalletBalance و Email

//.2 سفارش: نمایانگر سفارشهای مشتری با فیلدها
//ی مانند ،Product ،CustomerID ،OrderID//.OrderDate و Quantity، TotalAmount
