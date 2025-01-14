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
            // Initialize the user list with seed user
            _users = new List<User>
            {
                new User { Username = SeedUser, Password = SeedPassword, PreferredCurrency = "USD", Balance = SeedBalance }
            };
        }

        // Authentication of the user
        public bool AuthenticateUser(User user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return false;
            }

            var foundUser = _users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
            if (foundUser != null)
            {
                _authenticatedUser = foundUser;  
                return true;
            }

            return false;
        }

        // Implements Logout functionality
        public void Logout()
        {
            _authenticatedUser = null;
        }

        public User GetAuthenticatedUser()
        {
            return _authenticatedUser;
        }

        // Check if a user is logged in
        public bool IsUserAuthenticated()
        {
            return _authenticatedUser != null; 
        }
    }
}
