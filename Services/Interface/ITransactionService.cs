using ExpenwiseTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenwiseTracker.Services.Interface
{
    public interface ITransactionService
    {
        Task AddTransaction(Transaction transaction);

        Task<List<Transaction>> RetrieveAllTransactions();

        Task<double> CalculateUserBalance();
        Task<double> CalculateClearedDebts();

        Task<double> CalculateTotal(string type);
        Task<(double highest, double lowest)> GetStatistics(string type);
        Task<List<Transaction>> GetTopTransactions(int count);
        Task<List<(string Month, double Inflow, double Outflow, double Debt)>> GetMonthlyData();

    }
}
