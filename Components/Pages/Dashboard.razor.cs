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

        // Variables for chart data and labels
        private List<ChartSeries> Series = new List<ChartSeries>();
        private string[] XAxisLabels;

        // User data variables
        private int selectedIndex = -1;
        private List<Transaction> topTransactions = new List<Transaction>();
        private string userCurrency = string.Empty;
        private string username = string.Empty;

        // Chart data and labels
        private double[] data; 
        private string[] labels; 
        private Position LegendPosition { get; set; } = Position.Bottom;

        #region OnInitialized
        // OnInitializedAsync lifecycle method to load user preferences and dashboard data
        protected override async Task OnInitializedAsync()
        {
            // Fetch user preferences from session storage
            userCurrency = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "preferredCurrency");
            username = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "Username");

            await DashboardData();
        }
        #endregion

        #region DashboardData
        // Method to fetch and calculate the data needed for the dashboard
        private async Task DashboardData()
        {
            totalInflow = await TransactionService.CalculateTotal("Credit");
            totalOutflow = await TransactionService.CalculateTotal("Debit");
            totalDebts = await TransactionService.CalculateTotal("Debt");
            clearedDebts = await TransactionService.CalculateClearedDebts();

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
        #endregion
    }
}