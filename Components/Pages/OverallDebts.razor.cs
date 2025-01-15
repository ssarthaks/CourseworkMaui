using ExpenwiseTracker.Model;
using Microsoft.JSInterop;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class OverallDebts
    {
        // Data fields for storing transactions and other relevant data
        private List<Transaction> transactions = new();
        private List<Transaction> filteredTransactions = new();
        private List<Transaction> paginatedTransactions = new();

        private double currentBalance;

        private string searchName = string.Empty;
        private DateTime? selectedDate = null;
        private string sortOrder = "desc";
        private bool isPaidFilter = false;
        private string sortDueDateOrder = "asc";

        private int totalDebtCount;
        private int paidDebtCount;
        private int remainingDebtCount;

        private string userCurrency = string.Empty;

        private int currentPage = 1;
        private const int itemsPerPage = 5;

        // Calculate the total number of pages based on filtered transactions
        private int totalPages => (int)Math.Ceiling((double)filteredTransactions.Count / itemsPerPage);

        #region OnInitializedAsync
        // This method is invoked when the component is initialized
        protected override async Task OnInitializedAsync()
        {
            userCurrency = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "preferredCurrency");
            currentBalance = await TransactionService.CalculateUserBalance();
            transactions = DatabaseService.RetrieveAllTransactions();
            ApplyFilters();
        }
        #endregion

        #region GetTransactionTypeClass
        // This method determines the CSS class based on transaction type (e.g., "debt" or neutral type)
        private string GetTransactionTypeClass(string type) =>
            type.ToLower() switch
            {
                "debt" => "type-debt",
                _ => "type-neutral"
            };
        #endregion

        #region ApplyFilters
        // This method applies filters based on search criteria, date, and sorting order
        private void ApplyFilters()
        {
            filteredTransactions = transactions.Where(t => t.Type == "Debt").ToList();

            // Apply search filter by name
            if (!string.IsNullOrWhiteSpace(searchName))
            {
                filteredTransactions = filteredTransactions.Where(t => t.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Apply filter by selected date
            if (selectedDate.HasValue)
            {
                filteredTransactions = filteredTransactions.Where(t => t.Date >= selectedDate.Value.Date).ToList();
            }

            // Apply filter by Paid status
            if (isPaidFilter)
            {
                filteredTransactions = filteredTransactions.Where(t => t.IsPaid).ToList();
            }

            // Apply sorting by Date
            filteredTransactions = sortOrder switch
            {
                "asc" => filteredTransactions.OrderBy(t => t.Date).ToList(),
                "desc" => filteredTransactions.OrderByDescending(t => t.Date).ToList(),
                _ => filteredTransactions
            };

            // Apply sorting by DueDate if sortDueDateOrder is set
            filteredTransactions = sortDueDateOrder switch
            {
                "asc" => filteredTransactions.OrderBy(t => t.DueDate).ToList(),
                "desc" => filteredTransactions.OrderByDescending(t => t.DueDate).ToList(),
                _ => filteredTransactions
            };

            CalculateDebtCounts();
            UpdatePagination();
        }
        #endregion

        #region ClearFilters
        // This method clears all filters
        private void ClearFilters()
        {
            searchName = string.Empty;
            selectedDate = null;
            isPaidFilter = false;
            sortOrder = "desc";
            sortDueDateOrder = "asc";

            ApplyFilters();
        }

        #endregion

        #region CalculateDebtCounts
        // This method calculates total, paid, and remaining debt counts based on filtered transactions
        private void CalculateDebtCounts()
        {
            totalDebtCount = filteredTransactions.Count;
            paidDebtCount = filteredTransactions.Count(t => t.IsPaid);
            remainingDebtCount = totalDebtCount - paidDebtCount;
        }
        #endregion

        #region Pagination methods
        // This method updates the paginated transactions based on the current page and items per page
        private void UpdatePagination()
        {
            int skip = (currentPage - 1) * itemsPerPage;
            paginatedTransactions = filteredTransactions.Skip(skip).Take(itemsPerPage).ToList();
        }

        // This method increments the page number and updates the pagination
        private void NextPage()
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                UpdatePagination();
            }
        }

        // This method decrements the page number and updates the pagination
        private void PreviousPage()
        {
            if (currentPage > 1)
            {
                currentPage--;
                UpdatePagination();
            }
        }

        // This method checks whether the "Next" button should be disabled based on the current page
        private bool IsNextButtonDisabled() => currentPage >= totalPages;

        // This method checks whether the "Previous" button should be disabled based on the current page
        private bool IsPreviousButtonDisabled() => currentPage <= 1;

        #endregion

        #region ClearDebt
        // This method marks a debt as paid and recalculates the balance and debt counts
        private async Task ClearDebt(Transaction transaction)
        {
            transaction.IsPaid = true;
            DatabaseService.EstablishConnection().Update(transaction);

            currentBalance = await TransactionService.CalculateUserBalance();

            CalculateDebtCounts();
            transactions = DatabaseService.RetrieveAllTransactions();
            ApplyFilters();
        }
        #endregion

        #region ExportToCsv
        // This method generates a CSV of filtered transactions and triggers a download
        private async Task ExportToCsv()
        {
            var csvContent = "ID,Date,Title,Type,Amount,Tag,Source,DueDate,IsPaid\n" +
                string.Join("\n", filteredTransactions.Select(t =>
                    $"{t.Id},{t.Date:MM/dd/yyyy},{t.Name},{t.Type},{t.Amount},{t.Tag},{t.Source},{t.DueDate:MM/dd/yyyy},{t.IsPaid}"));

            var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
            var csvFileName = $"Debts_{DateTime.Now:yyyyMMddHHmmss}.csv";

            await JSRuntime.InvokeVoidAsync("downloadFile", csvFileName, csvBytes);
        }
        #endregion
    }
}
