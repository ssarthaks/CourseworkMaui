using ExpenwiseTracker.Model;
namespace ExpenwiseTracker.Components.Pages
{
    public partial class AddTransaction
    {
        private Transaction transaction = new Transaction();
        private string? NotifyUser;
        private string? NotifyUserClass;
        private string? selectedTag;
        private string? customTag;
        private List<Tag> availableTags = new List<Tag>();

        private double userBalance;

        #region OnInitializedAsync Method
        // Method to fetch initial data like tags and balance when the component is initialized
        protected override async Task OnInitializedAsync()
        {
            try
            {
                availableTags = await TagService.GetAllTags();
                userBalance = await TransactionService.CalculateUserBalance();
            }
            catch (Exception ex)
            {
                NotifyUser = "Error fetching initial data: " + ex.Message;
                NotifyUserClass = "alert-danger";
                Console.WriteLine($"Error fetching initial data: {ex.Message}");
            }
        }
        #endregion

        #region SubmitTransaction Method
        // This method handles submit transaction
        private async Task SubmitTransaction()
        {
            try
            {
                transaction.Date = DateTime.Now;

                userBalance = await TransactionService.CalculateUserBalance();

                if (transaction.Type == "Debit" && transaction.Amount > userBalance)
                {
                    NotifyUser = "Insufficient balance!";
                    NotifyUserClass = "alert-danger";
                    return;
                }

                if (transaction.Type == "Debt")
                {
                    transaction.IsPaid = false;
                }

                if (selectedTag == "Other" && !string.IsNullOrWhiteSpace(customTag))
                {
                    var newTag = new Tag { Name = customTag };
                    await TagService.AddTag(newTag);
                    transaction.Tag = customTag;

                    availableTags.Add(newTag);
                }
                else if (selectedTag != "Other")
                {
                    transaction.Tag = selectedTag;
                }
                else
                {
                    NotifyUser = "Custom tag is required!";
                    NotifyUserClass = "alert-danger";
                    return;
                }

                await TransactionService.AddTransaction(transaction);

                NotifyUser = "Transaction added successfully!";
                NotifyUserClass = "alert-success";

                userBalance = await TransactionService.CalculateUserBalance();

                ResetForm();
            }
            catch (Exception ex)
            {
                NotifyUser = "Error adding transaction: " + ex.Message;
                NotifyUserClass = "alert-danger";
                Console.WriteLine($"Error adding transaction: {ex.Message}"); 
            }
        }
        #endregion

        #region ResetForm Method
        // Method to reset form fields after successful transaction
        private void ResetForm()
        {
            transaction = new Transaction();
            selectedTag = null;
            customTag = null;
        }
        #endregion

        #region IsSubmitDisabled Method
        private bool IsSubmitDisabled() => false;
        #endregion
    }
}
