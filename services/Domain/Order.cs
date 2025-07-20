using RedMaskFramework.DDD;

namespace Domain;

public class Order : IAggregateRoot
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public AppUser Customer { get; set; }
    public string Product { get; set; }
    public DateTime OrderDate { get; set; }
    public int Quantity { get; set; }
    public long TotalAmount { get; set; }
}