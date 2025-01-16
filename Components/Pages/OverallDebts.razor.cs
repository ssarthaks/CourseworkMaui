using ExpenwiseTracker.Model;
using Microsoft.JSInterop;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class OverallDebts
    {
        private List<Transaction> transactions = new();
        private List<Transaction> filteredTransactions = new();
        private List<Transaction> paginatedTransactions = new();

        private double currentBalance;

        private string searchName = string.Empty;
        private DateTime? selectedDate = null;
        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private string sortOrder = "desc";
        private bool isPaidFilter = false;
        private bool isUnPaidFilter = false;
        private string? sortDueDateOrder;

        private int totalDebtCount;
        private int paidDebtCount;
        private int remainingDebtCount;

        private string userCurrency = string.Empty;
        private string insufficientBalanceMessage = string.Empty;

        private int currentPage = 1;
        private const int itemsPerPage = 5;

        private int totalPages => (int)Math.Ceiling((double)filteredTransactions.Count / itemsPerPage);

        #region OnInitializedAsync
        protected override async Task OnInitializedAsync()
        {
            try
            {
                userCurrency = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "preferredCurrency");
                currentBalance = await TransactionService.CalculateUserBalance();
                transactions = DatabaseService.RetrieveAllTransactions();
                ApplyFilters();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred during initialization. Please try again later.");
                LogError(ex);
            }
        }
        #endregion

        #region GetTransactionTypeClass
        // This method gets the transaction type class
        private string GetTransactionTypeClass(string type) =>
            type.ToLower() switch
            {
                "debt" => "type-debt",
                _ => "type-neutral"
            };
        #endregion

        #region ApplyFilters
        // Filter method
        private void ApplyFilters()
        {
            try
            {
                filteredTransactions = transactions.Where(t => t.Type == "Debt").ToList();

                if (!string.IsNullOrWhiteSpace(searchName))
                {
                    filteredTransactions = filteredTransactions.Where(t => t.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (selectedDate.HasValue)
                {
                    filteredTransactions = filteredTransactions.Where(t => t.Date >= selectedDate.Value.Date).ToList();
                }

                if (isPaidFilter)
                {
                    filteredTransactions = filteredTransactions.Where(t => t.IsPaid).ToList();
                }

                if (isUnPaidFilter)
                {
                    filteredTransactions = filteredTransactions.Where(t => !t.IsPaid).ToList();
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

                filteredTransactions = sortOrder switch
                {
                    "asc" => filteredTransactions.OrderBy(t => t.Date).ToList(),
                    "desc" => filteredTransactions.OrderByDescending(t => t.Date).ToList(),
                    _ => filteredTransactions
                };

                filteredTransactions = sortDueDateOrder switch
                {
                    "asc" => filteredTransactions.OrderBy(t => t.DueDate).ToList(),
                    "desc" => filteredTransactions.OrderByDescending(t => t.DueDate).ToList(),
                    _ => filteredTransactions
                };

                CalculateDebtCounts();
                UpdatePagination();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred while applying filters. Please try again.");
                LogError(ex);
            }
        }
        #endregion

        #region ClearFilters
        // This method clears all filters
        private void ClearFilters()
        {
            try
            {
                searchName = string.Empty;
                selectedDate = null;
                isPaidFilter = false;
                isUnPaidFilter = false;
                startDate = null;
                endDate = null;
                sortOrder = "desc";
                sortDueDateOrder = null;

                ApplyFilters();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred while clearing the filters. Please try again.");
                LogError(ex);
            }
        }
        #endregion

        #region CalculateDebtCounts
        // Pagination calculate debt counts method
        private void CalculateDebtCounts()
        {
            try
            {
                totalDebtCount = filteredTransactions.Count;
                paidDebtCount = filteredTransactions.Count(t => t.IsPaid);
                remainingDebtCount = totalDebtCount - paidDebtCount;
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred while calculating the debt counts. Please try again.");
                LogError(ex);
            }
        }
        #endregion

        #region UpdatePagination
        // Pagination update method
        private void UpdatePagination()
        {
            try
            {
                int skip = (currentPage - 1) * itemsPerPage;
                paginatedTransactions = filteredTransactions.Skip(skip).Take(itemsPerPage).ToList();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred while updating pagination. Please try again.");
                LogError(ex);
            }
        }
        #endregion

        #region NextPage
        // Pagination next page method
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
                ShowErrorMessage("An error occurred while navigating to the next page. Please try again.");
                LogError(ex);
            }
        }
        #endregion

        #region PreviousPage
        // Pagination previous page method
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
                ShowErrorMessage("An error occurred while navigating to the previous page. Please try again.");
                LogError(ex);
            }
        }
        #endregion

        #region
        // The method is next button disabled
        private bool IsNextButtonDisabled() => currentPage >= totalPages;
        private bool IsPreviousButtonDisabled() => currentPage <= 1;

        #endregion

        #region ClearDebt
        // This method clears the debt after verifying the user balance
        private async Task ClearDebt(Transaction transaction)
        {
            try
            {
                insufficientBalanceMessage = string.Empty;

                currentBalance = await TransactionService.CalculateUserBalance();

                if (currentBalance < transaction.Amount)
                {
                    insufficientBalanceMessage = $"Insufficient balance to clear the debt of {userCurrency} {transaction.Amount:F2}!";
                    return;
                }

                transaction.IsPaid = true;
                DatabaseService.EstablishConnection().Update(transaction);

                currentBalance -= transaction.Amount;

                CalculateDebtCounts();
                transactions = DatabaseService.RetrieveAllTransactions();
                ApplyFilters();

                insufficientBalanceMessage = string.Empty; 
                Console.WriteLine($"Debt of {userCurrency} {transaction.Amount:F2} cleared successfully!");
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred while clearing the debt. Please try again.");
                LogError(ex);
            }
        }

        #endregion

        #region ExportToCsv
        // This method exports the csv file
        private async Task ExportToCsv()
        {
            try
            {
                var csvContent = "ID,Date,Title,Type,Amount,Tag,Source,DueDate,IsPaid\n" +
                    string.Join("\n", filteredTransactions.Select(t =>
                        $"{t.Id},{t.Date:MM/dd/yyyy},{t.Name},{t.Type},{t.Amount},{t.Tag},{t.Source},{t.DueDate:MM/dd/yyyy},{t.IsPaid}"));

                var csvBytes = System.Text.Encoding.UTF8.GetBytes(csvContent);
                var csvFileName = $"Debts_{DateTime.Now:yyyyMMddHHmmss}.csv";

                await JSRuntime.InvokeVoidAsync("downloadFile", csvFileName, csvBytes);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("An error occurred while exporting to CSV. Please try again.");
                LogError(ex);
            }
        }
        #endregion

        #region HelperMethods
        private void ShowErrorMessage(string message)
        {
            Console.WriteLine(message);
        }

        private void LogError(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }
        #endregion
    }
}
