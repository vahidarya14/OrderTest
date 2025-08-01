namespace Domain;

public class Wallet
{
    public long Id { get; set; }
    public long Balance => FreeBalance - BlockedAmount;
    public long FreeBalance { get; set; }
    public long BlockedAmount { get; set; }
    public long UserId { get; set; }
    public WalletSetting Setting { get; set; }

    public bool AllowWithdrawl(long newWithdrawl) => Balance >= Setting.MinBalanceWithdrawlThreshold - newWithdrawl;
}

public class WalletSetting
{
    public long MinBalanceWithdrawlThreshold { get; set; }
}