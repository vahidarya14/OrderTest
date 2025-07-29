namespace Domain;

public class Transaction
{
    public long Id { get; set; }
    public DateTime DueDate { get; set; }
    public long Amount { get; set; }
    public string Reference { get; set; }
    public TransactionType TransactionType { get; set; }
    public TransactionStatus Status { get; set; }
}
public enum TransactionType
{
    Payment,
    Withdrawl,
    TopUp
}
public enum TransactionStatus
{
    Pending,
    Failled,
    Completed
}