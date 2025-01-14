using ExpenwiseTracker.Model;
using Microsoft.JSInterop;
using MudBlazor;
using ExpenwiseTracker.Services;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class Dashboard
    {
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
        private List<ChartSeries> Series = new List<ChartSeries>();
        private string[] XAxisLabels;
        private int selectedIndex = -1;

        private List<Transaction> topTransactions = new List<Transaction>();
        private string userCurrency = string.Empty;
        private string username = string.Empty;

        #region OnInitialized
        protected override async Task OnInitializedAsync()
        {
            userCurrency = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "preferredCurrency");
            username = await JSRuntime.InvokeAsync<string>("sessionStorage.getItem", "Username");
            // Load dashboard data
            await LoadDashboardData();
        }
        #endregion

        private async Task LoadDashboardData()
        {
            totalInflow = await TransactionService.CalculateTotal("Credit");
            totalOutflow = await TransactionService.CalculateTotal("Debit");
            totalDebts = await TransactionService.CalculateTotal("Debt");
            clearedDebts = await TransactionService.CalculateClearedDebts();

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
    }
}
