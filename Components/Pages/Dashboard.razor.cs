using ExpenwiseTracker.Model;
using Microsoft.JSInterop;
using MudBlazor;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class Dashboard
    {
        // Variables to hold various dashboard data values
        private double currentBalance;
        private double totalInflow;
        private double totalOutflow;
        private double totalDebts;
        private double clearedDebts;
        private double highestInflow;
        private double lowestInflow;
        private double highestOutflow;
        private double lowestOutflow;
        private double highestDebt;
        private double lowestDebt;
        private double remainingDebt;

        private DateTime? startDate = null;
        private DateTime? endDate = null;
        private List<Transaction> topRemainingDebts = new();

        // Variables for chart data and labels
        private List<ChartSeries> Series = new List<ChartSeries>();
        private string[] XAxisLabels;

        // User data variables
        private int selectedIndex = -1;
        private List<Transaction> topTransactions = new List<Transaction>();
        private string userCurrency = string.Empty;

        // Chart data and labels
        private double[] data;
        private string[] labels;
        private Position LegendPosition { get; set; } = Position.Bottom;

        #region OnInitialized
        // OnInitializedAsync lifecycle method to load user preferences and dashboard data
        protected override async Task OnInitializedAsync()
        {
            try
            {
                userCurrency = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "preferredCurrency");

                await GetTopRemainingDebts();
                await DashboardData();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during initialization: {ex.Message}");
            }
        }
        #endregion

        #region DashboardData
        // Method to fetch and calculate the data needed for the dashboard
        private async Task DashboardData()
        {
            try
            {
                totalInflow = await TransactionService.CalculateTotal("Credit");
                totalOutflow = await TransactionService.CalculateTotal("Debit");
                totalDebts = await TransactionService.CalculateTotal("Debt");
                clearedDebts = await TransactionService.CalculateClearedDebts();
                remainingDebt = totalDebts - clearedDebts;

                data = new double[] { totalInflow, totalOutflow, totalDebts };
                labels = new string[]
                {
                    $"Credit: {@userCurrency}.{totalInflow.ToString("F2")}",
                    $"Debit: {@userCurrency}.{totalOutflow.ToString("F2")}",
                    $"Debt: {@userCurrency}.{totalDebts.ToString("F2")}"
                };

                (highestInflow, lowestInflow) = await TransactionService.GetStatistics("Credit");
                (highestOutflow, lowestOutflow) = await TransactionService.GetStatistics("Debit");
                (highestDebt, lowestDebt) = await TransactionService.GetStatistics("Debt");

                topTransactions = await TransactionService.GetTopTransactions(5);

                currentBalance = totalInflow - totalOutflow + totalDebts - clearedDebts;

                var monthlyData = await TransactionService.GetMonthlyData();
                Series = new List<ChartSeries>
                {
                    new ChartSeries { Name = "Inflow", Data = monthlyData.Select(m => m.Inflow).ToArray() },
                    new ChartSeries { Name = "Outflow", Data = monthlyData.Select(m => m.Outflow).ToArray() },
                    new ChartSeries { Name = "Debt", Data = monthlyData.Select(m => m.Debt).ToArray() }
                };
                XAxisLabels = monthlyData.Select(m => m.Month).ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching dashboard data: {ex.Message}");
            }
        }
        #endregion

        #region FilterMethods
        // Method to filter remaining debts based on the selected date range
        private async Task GetTopRemainingDebts()
        {
            try
            {
                if (startDate == null || endDate == null)
                {
                    var transactions = DatabaseService.RetrieveAllTransactions();

                    var unpaidDebts = transactions
                        .Where(t => t.Type == "Debt" && !t.IsPaid)
                        .OrderBy(t => t.DueDate)
                        .Take(5)
                        .ToList();

                    topRemainingDebts = unpaidDebts;
                }
                else
                {
                    var transactions = DatabaseService.RetrieveAllTransactions();

                    var unpaidDebts = transactions
                        .Where(t => t.Type == "Debt" && !t.IsPaid)
                        .Where(t => t.DueDate >= startDate.Value.Date && t.DueDate <= endDate.Value.Date)
                        .OrderBy(t => t.DueDate)
                        .Take(5)
                        .ToList();

                    topRemainingDebts = unpaidDebts;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching remaining debts: {ex.Message}");
            }
        }

        //This method applies filter
        private async Task ApplyFilter()
        {
            try
            {
                await GetTopRemainingDebts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying filter: {ex.Message}");
            }
        }

        //This method clears the filter options
        private void ClearFilters()
        {
            try
            {
                startDate = null;
                endDate = null;

                GetTopRemainingDebts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing filters: {ex.Message}");
            }
        }
        #endregion
    }
}
