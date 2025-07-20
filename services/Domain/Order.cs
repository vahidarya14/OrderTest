namespace Domain;

public class Order
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public AppUser Customer { get; set; }
    public string Product { get; set; }
    public DateTime OrderDate { get; set; }
    public int Quantity { get; set; }
    public long TotalAmount { get; set; }
}

//.1 مشتری )حساب(: نمایانگر یک مشتری با فیلدها
//ی مانند ،LastName ،FirstName ،CustomerID//.Role و WalletBalance و Email

//.2 سفارش: نمایانگر سفارشهای مشتری با فیلدها
//ی مانند ،Product ،CustomerID ،OrderID//.OrderDate و Quantity، TotalAmount
