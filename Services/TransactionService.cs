using ExpenwiseTracker.Model;
using SQLite;
using System.Linq;

namespace ExpenwiseTracker.Services
{
    public class TransactionService
    {
        private readonly SQLiteConnection _connection;

        public TransactionService(DatabaseService databaseService)
        {
            _connection = databaseService.EstablishConnection();
        }

        // Add Transaction to DB asynchronously
        public async Task AddTransactionAsync(Transaction transaction)
        {
            await Task.Run(() => _connection.Insert(transaction));
        }

        // Get all transactions from the database asynchronously
        public async Task<List<Transaction>> RetrieveAllTransactions()
        {
            return await Task.Run(() => _connection.Table<Transaction>().ToList());
        }

        // Get the user balance by summing all "Credit" and "Debit" transactions
        public async Task<double> CalculateUserBalanceAsync()
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().ToList());

            // Ensure transactions is not null and contains elements
            if (transactions == null || transactions.Count == 0)
            {
                return 0; // No transactions found, so balance is 0
            }

            // Calculate balance: Sum of Credit minus Debit (or debt if applicable)
            double balance = transactions
                .Where(t => t.Type == "Credit")
                .Sum(t => t.Amount) -
                transactions
                .Where(t => t.Type == "Debit" || t.Type == "Debt")
                .Sum(t => t.Amount);

            return balance;
        }
    }
}
