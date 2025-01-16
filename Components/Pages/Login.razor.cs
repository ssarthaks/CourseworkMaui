using ExpenwiseTracker.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class Login
    {
        private LoginModel loginModel = new LoginModel();

        private string? errorMessage;

        private List<string> currencies = new List<string> { "USD", "EUR", "NPR", "INR" };

        #region AuthenticateUser Method

        // This method is triggered when the user submits the login form
        private async Task AuthenticateUser()
        {
            errorMessage = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(loginModel.Username) ||
                    string.IsNullOrWhiteSpace(loginModel.Password) ||
                    string.IsNullOrWhiteSpace(loginModel.PreferredCurrency))
                {
                    errorMessage = "Please fill out all the fields.";
                    return;
                }

                bool isAuthenticated = UserService.UserAuthentication(new User
                {
                    Username = loginModel.Username,
                    Password = loginModel.Password,
                    PreferredCurrency = loginModel.PreferredCurrency
                });

                if (isAuthenticated)
                {
                    await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", "preferredCurrency", loginModel.PreferredCurrency);

                    NavigationManager.NavigateTo("/");
                }
                else
                {
                    errorMessage = "Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                errorMessage = "An error occurred during login. Please try again later.";
                LogError(ex); 
            }
        }

        #endregion

        #region Login Model
        // Model class to represent the login form data
        private class LoginModel
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string PreferredCurrency { get; set; }
        }

        #endregion

        #region Helper Methods
        // Method to log errors for debugging
        private void LogError(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
        }

        #endregion
    }
}
