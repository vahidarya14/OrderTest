using Domain;
using Persistance;
using Persistance.Data;

namespace Application
{
    public class WalletService(AppDbContext db)
    {
        public void AddTransaction(Transaction transaction) { }

        public void ChnageTransactionStatus(long walletId, long transactionId, TransactionStatus status)
        {

            //change transaction and add wallet balanse if is payment
        }


    }
}