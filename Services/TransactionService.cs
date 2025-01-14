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

        public async Task AddTransaction(Transaction transaction)
        {
            await Task.Run(() => _connection.Insert(transaction));
        }

        public async Task<List<Transaction>> RetrieveAllTransactions()
        {
            return await Task.Run(() => _connection.Table<Transaction>().ToList());
        }

        public async Task<double> CalculateUserBalance()
        {
            var transactions = await RetrieveAllTransactions();

            if (transactions == null || !transactions.Any())
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

        public async Task<double> CalculateTotal(string type)
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().Where(t => t.Type == type).ToList());
            return transactions.Sum(t => t.Amount);
        }

        public async Task<double> CalculateClearedDebts()
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().Where(t => t.Type == "Debt" && t.IsPaid).ToList());
            return transactions.Sum(t => t.Amount);
        }

        public async Task<(double highest, double lowest)> GetStatistics(string type)
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().Where(t => t.Type == type).ToList());
            if (!transactions.Any())
                return (0, 0);

            return (transactions.Max(t => t.Amount), transactions.Min(t => t.Amount));
        }

        public async Task<List<Transaction>> GetTopTransactions(int count)
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().OrderByDescending(t => t.Amount).Take(count).ToList());
            return transactions;
        }

        public async Task<List<(string Month, double Inflow, double Outflow, double Debt)>> GetMonthlyData()
        {
            var transactions = await RetrieveAllTransactions();

            return transactions
                .GroupBy(t => t.Date.ToString("MMM"))
                .Select(g => (
                    Month: g.Key,
                    Inflow: g.Where(t => t.Type == "Credit").Sum(t => t.Amount),
                    Outflow: g.Where(t => t.Type == "Debit").Sum(t => t.Amount),
                    Debt: g.Where(t => t.Type == "Debt").Sum(t => t.Amount)
                ))
                .OrderBy(g => DateTime.ParseExact(g.Month, "MMM", null))
                .ToList();
        }
    }
}
