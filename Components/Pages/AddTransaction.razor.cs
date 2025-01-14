using ExpenwiseTracker.Model;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class AddTransaction
    {
        private Transaction transaction = new Transaction();
        private string NotifyUser;
        private string NotifyUserClass;
        private string selectedTag;
        private string customTag;
        private List<Tag> availableTags = new List<Tag>();

        // This will hold the current balance of the user
        private double userBalance;

        // On initialization, fetch available tags and user's balance
        protected override async Task OnInitializedAsync()
        {
            try
            {
                availableTags = await TagService.GetAllTags(); // Use injected TagService
                userBalance = await TransactionService.CalculateUserBalance(); // Fetch balance
            }
            catch (Exception ex)
            {
                NotifyUser = "Error fetching data: " + ex.Message;
                NotifyUserClass = "alert-danger";
            }
        }

        private async Task SubmitTransaction()
        {
            try
            {
                transaction.Date = DateTime.Now;

                // Validate if the transaction amount exceeds the balance before submitting
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

                // If "Other" is selected, add the custom tag to the transaction and save it
                if (selectedTag == "Other")
                {
                    if (!string.IsNullOrWhiteSpace(customTag))
                    {
                        var newTag = new Tag { Name = customTag };
                        await TagService.AddTag(newTag); // Use injected TagService
                        transaction.Tag = customTag;

                        // Optionally, add the custom tag to availableTags list for future use
                        availableTags.Add(newTag);
                    }
                    else
                    {
                        NotifyUser = "Custom tag is required!";
                        NotifyUserClass = "alert-danger";
                        return;
                    }
                }
                else
                {
                    transaction.Tag = selectedTag; // Use selected tag
                }

                // Add the transaction to the database
                await TransactionService.AddTransaction(transaction);

                // Refresh the user's balance and reset the form
                userBalance = await TransactionService.CalculateUserBalance();

                NotifyUser = "Transaction added successfully!";
                NotifyUserClass = "alert-success";

                transaction = new Transaction(); // Clear form fields

                selectedTag = null; // Reset tag selection
                customTag = null; // Reset custom tag field
            }
            catch (Exception ex)
            {
                NotifyUser = "Error adding transaction: " + ex.Message;
                NotifyUserClass = "alert-danger";
            }
        }

        private bool IsSubmitDisabled()
        {
            return false;
        }

    }
}