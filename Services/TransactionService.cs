using ExpenwiseTracker.Model;
using ExpenwiseTracker.Services.Interface;
using SQLite;
using System.Linq;

namespace ExpenwiseTracker.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly SQLiteConnection _connection;

        public TransactionService(DbConnectionService databaseService)
        {
            _connection = databaseService.EstablishConnection();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            await Task.Run(() => _connection.Insert(transaction));
        }

        public async Task<List<Transaction>> RetrieveAllTransactions()
        {
            return await Task.Run(() => _connection.Table<Transaction>().ToList());
        }

        public async Task<double> CalculateUserBalanceAsync()
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().ToList());

            if (transactions == null || transactions.Count == 0)
            {
                return 0;
            }

            double balance = transactions
                .Where(t => (t.Type == "Credit" || t.Type == "Debt") && !t.IsPaid)
                .Sum(t => t.Amount) -
                transactions
                .Where(t => t.Type == "Debit")
                .Sum(t => t.Amount);

            return balance;
        }
    }

}
