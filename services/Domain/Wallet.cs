namespace Domain;

public class Wallet
{
    public long Id { get; set; }
    public long Balance { get; set; }
    public long UserId { get; set; }
    public Walletsetting Walletsetting { get; set; }
}

public class Walletsetting
{
    public long MinTreshold { get; set; }
}