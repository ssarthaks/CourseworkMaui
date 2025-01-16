using ExpenwiseTracker.Model;
using Microsoft.JSInterop;
using System.Text;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class TransactionsHistory
    {
        // Variables used to store the fetched data
        private List<Transaction> transactions = new();
        private List<Transaction> filteredTransactions = new();
        private List<Transaction> paginatedTransactions = new();
        private int totalTransactions;
        private int totalInflowCount;
        private int totalOutflowCount;
        private int totalDebtCount;
        private int totalPages => (int)Math.Ceiling((double)filteredTransactions.Count / itemsPerPage);
        private string searchName = string.Empty;
        private string selectedType = string.Empty;
        private DateTime? selectedDate = null;
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string sortOrder = "desc";
        private int currentPage = 1;
        private const int itemsPerPage = 5;
        private string userCurrency = string.Empty;
        private double currentBalance = 0;
        private string errorMessage = string.Empty; 

        #region On Initialized Method
        // Code that runs on program initialization
        protected override async Task OnInitializedAsync()
        {
            try
            {
                userCurrency = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "preferredCurrency");
                currentBalance = await TransactionService.CalculateUserBalance();

                transactions = await TransactionService.RetrieveAllTransactions();
                totalTransactions = transactions.Count;
                totalInflowCount = transactions.Count(t => t.Type.Equals("Credit", StringComparison.OrdinalIgnoreCase));
                totalOutflowCount = transactions.Count(t => t.Type.Equals("Debit", StringComparison.OrdinalIgnoreCase));
                totalDebtCount = transactions.Count(t => t.Type.Equals("Debt", StringComparison.OrdinalIgnoreCase));

                ApplyFilters();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error initializing transactions: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }
        #endregion

        #region Filter Methods
        // This method applies filters to the transactions
        private void ApplyFilters()
        {
            try
            {
                filteredTransactions = transactions;

                if (!string.IsNullOrWhiteSpace(searchName))
                {
                    filteredTransactions = filteredTransactions
                        .Where(t => t.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (!string.IsNullOrEmpty(selectedType))
                {
                    filteredTransactions = filteredTransactions
                        .Where(t => t.Type.Equals(selectedType, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    filteredTransactions = filteredTransactions
                        .Where(t => t.Date.Date >= startDate.Value.Date && t.Date.Date <= endDate.Value.Date).ToList();
                }
                else if (startDate.HasValue)
                {
                    filteredTransactions = filteredTransactions
                        .Where(t => t.Date.Date >= startDate.Value.Date).ToList();
                }
                else if (endDate.HasValue)
                {
                    filteredTransactions = filteredTransactions
                        .Where(t => t.Date.Date <= endDate.Value.Date).ToList();
                }

                filteredTransactions = sortOrder == "asc"
                    ? filteredTransactions.OrderBy(t => t.Date).ToList()
                    : filteredTransactions.OrderByDescending(t => t.Date).ToList();

                UpdatePagination();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error applying filters: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        // This method clears all filters
        private void ClearFilters()
        {
            try
            {
                searchName = string.Empty;
                selectedType = null;
                sortOrder = "desc";
                startDate = null;
                endDate = null;
                ApplyFilters();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error clearing filters: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }
        #endregion

        #region Pagination Methods
        private void UpdatePagination()
        {
            try
            {
                int skip = (currentPage - 1) * itemsPerPage;
                paginatedTransactions = filteredTransactions.Skip(skip).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = $"Error updating pagination: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        private void NextPage()
        {
            try
            {
                if (currentPage < totalPages)
                {
                    currentPage++;
                    UpdatePagination();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error navigating to the next page: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        private void PreviousPage()
        {
            try
            {
                if (currentPage > 1)
                {
                    currentPage--;
                    UpdatePagination();
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error navigating to the previous page: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }

        private bool IsNextButtonDisabled() => currentPage >= totalPages;

        private bool IsPreviousButtonDisabled() => currentPage <= 1;
        #endregion

        #region Get Transaction Type
        // This method returns the transaction type class
        private string GetTransactionTypeClass(string type)
        {
            try
            {
                return type.ToLower() switch
                {
                    "credit" => "type-inflow",
                    "debit" => "type-outflow",
                    "debt" => "type-debt",
                    _ => "type-neutral"
                };
            }
            catch (Exception ex)
            {
                errorMessage = $"Error determining transaction type class: {ex.Message}";
                Console.WriteLine(errorMessage);
                return "type-error";
            }
        }
        #endregion

        #region Export to CSV
        // This method exports to csv
        private async Task ExportToCsv()
        {
            try
            {
                var csv = new StringBuilder();
                csv.AppendLine("ID,Date,Title,Amount,Type,Tags,Notes");

                foreach (var transaction in filteredTransactions)
                {
                    csv.AppendLine($"{transaction.Id},{transaction.Date:MM/dd/yyyy},{transaction.Name},{transaction.Amount},{transaction.Type},{transaction.Tag},{transaction.Notes}");
                }

                await JSRuntime.InvokeVoidAsync("downloadFile", "transactions.csv", csv.ToString());
            }
            catch (Exception ex)
            {
                errorMessage = $"Error exporting transactions to CSV: {ex.Message}";
                Console.WriteLine(errorMessage);
            }
        }
        #endregion
    }
}
