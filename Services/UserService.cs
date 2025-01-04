using ExpenwiseTracker.Model;
using ExpenwiseTracker.Services.Interface;

namespace ExpenwiseTracker.Services
{
    public class UserService : IUserService
    {
        private List<User> _users;
        private User _authenticatedUser;

        // Seeded user credentials
        public const string SeedUser = "admin";
        public const string SeedPassword = "admin";
        public const double SeedBalance = 0;

        public UserService()
        {
            // Initialize the user list with a seeded user
            _users = new List<User>
            {
                new User { Username = SeedUser, Password = SeedPassword, PreferredCurrency = "USD", Balance = SeedBalance }
            };
        }

        // Implements the Login method from the IUserService interface
        public bool AuthenticateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return false;
            }

            var foundUser = _users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (foundUser != null)
            {
                _authenticatedUser = foundUser;  // Store the authenticated user
                return true;
            }

            return false;
        }

        // Add this method to your UserService for getting the current user after authentication
        public User GetAuthenticatedUser()
        {
            return _authenticatedUser; // Return the authenticated user
        }
    }
}