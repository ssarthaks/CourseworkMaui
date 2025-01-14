using ExpenwiseTracker.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ExpenwiseTracker.Components.Pages
{
    public partial class Login
    {
        // Model that binds to the form fields (Username, Password, and Preferred Currency)
        private LoginModel loginModel = new LoginModel();

        // Variable to store error message if authentication fails
        private string? errorMessage;

        // List of available currencies for the user to choose from
        private List<string> currencies = new List<string> { "USD", "EUR", "NPR", "INR" };

        #region AuthenticateUser Method

        // This method is triggered when the user submits the login form
        private async Task AuthenticateUser()
        {
            errorMessage = string.Empty; 

            if (string.IsNullOrWhiteSpace(loginModel.Username) ||
                string.IsNullOrWhiteSpace(loginModel.Password) ||
                string.IsNullOrWhiteSpace(loginModel.PreferredCurrency))
            {
                errorMessage = "Please fill out all the fields.";
                return;
            }

            if (UserService.AuthenticateUser(new User
            {
                Username = loginModel.Username,  
                Password = loginModel.Password,  
                PreferredCurrency = loginModel.PreferredCurrency  
            }))
            {
                await JSRuntime.InvokeVoidAsync("sessionStorage.setItem", "preferredCurrency", loginModel.PreferredCurrency);

                NavigationManager.NavigateTo("/");
            }
            else
            {
                errorMessage = "Invalid username or password.";
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
    }
}
