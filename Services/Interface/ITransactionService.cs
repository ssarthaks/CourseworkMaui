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
        Task AddTransactionAsync(Transaction transaction);

        Task<List<Transaction>> RetrieveAllTransactions();

        Task<double> CalculateUserBalanceAsync();
    }
}
