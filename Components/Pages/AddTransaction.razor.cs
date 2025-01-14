using ExpenwiseTracker.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        private double userBalance;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                availableTags = await TagService.GetAllTags();
                userBalance = await TransactionService.CalculateUserBalance();
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

                    availableTags.Add(newTag); // Add to local cache
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
                userBalance = await TransactionService.CalculateUserBalance();

                NotifyUser = "Transaction added successfully!";
                NotifyUserClass = "alert-success";

                ResetForm();
            }
            catch (Exception ex)
            {
                NotifyUser = "Error adding transaction: " + ex.Message;
                NotifyUserClass = "alert-danger";
            }
        }

        private void ResetForm()
        {
            transaction = new Transaction();
            selectedTag = null;
            customTag = null;
        }

        private bool IsSubmitDisabled() => false;
    }
}
