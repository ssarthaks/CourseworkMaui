using ExpenwiseTracker.Model;
using ExpenwiseTracker.Services.Interface;
using SQLite;

namespace ExpenwiseTracker.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly SQLiteConnection _connection;

        public TransactionService(DbConnectionService databaseService)
        {
            _connection = databaseService.EstablishConnection();
        }

        #region Public Methods

        #region AddTransaction
        // Adds a new transaction to the database.
        public async Task AddTransaction(Transaction transaction)
        {
            await Task.Run(() => _connection.Insert(transaction));
        }

        #endregion

        #region RetrieveAllTransactions
        // Retrieves all transactions from the database asynchronously.
        public async Task<List<Transaction>> RetrieveAllTransactions()
        {
            return await Task.Run(() => _connection.Table<Transaction>().ToList());
        }
        #endregion

        #region CalculateUserBalance
        // Calculates the user's current balance based on Credit, Debit, and Debt transactions.
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
        #endregion

        #region CalculateTotal
        // Calculates the total amount for a specific transaction type (e.g., "Credit", "Debit").
        public async Task<double> CalculateTotal(string type)
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().Where(t => t.Type == type).ToList());
            return transactions.Sum(t => t.Amount);
        }
        #endregion

        #region CalculateClearedDebts
        // Calculates the total amount of debts that have been marked as paid.
        public async Task<double> CalculateClearedDebts()
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().Where(t => t.Type == "Debt" && t.IsPaid).ToList());
            return transactions.Sum(t => t.Amount);
        }
        #endregion

        #region GetStatistics
        // Retrieves the highest and lowest transaction amounts for a given type (e.g., "Credit", "Debit").
        public async Task<(double highest, double lowest)> GetStatistics(string type)
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().Where(t => t.Type == type).ToList());
            if (!transactions.Any())
                return (0, 0);

            return (transactions.Max(t => t.Amount), transactions.Min(t => t.Amount));
        }
        #endregion

        #region GetTopTransactions
        // Retrieves the top N transactions ordered by amount (descending).
        public async Task<List<Transaction>> GetTopTransactions(int count)
        {
            var transactions = await Task.Run(() => _connection.Table<Transaction>().OrderByDescending(t => t.Amount).Take(count).ToList());
            return transactions;
        }
        #endregion

        #region GetMonthlyData
        // Retrieves monthly statistics for Inflows, Outflows, and Debts, grouped by month.
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
        #endregion

        #endregion
    }
}