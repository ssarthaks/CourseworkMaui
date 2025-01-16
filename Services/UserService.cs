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

        #region Authentication Methods

        #region AuthenticateUser
        // Authenticates a user based on the provided username and password.
        public bool UserAuthentication(User user)
        {
            try
            {
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user), "User object cannot be null.");
                }

                if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                {
                    throw new ArgumentException("Username and Password cannot be empty.");
                }

                var foundUser = _users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);
                if (foundUser != null)
                {
                    _authenticatedUser = foundUser;
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UserAuthentication: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Logout
        // This method logs out the authenticated user by clearing the session.
        public void Logout()
        {
            try
            {
                if (_authenticatedUser == null)
                {
                    throw new InvalidOperationException("No user is currently authenticated.");
                }

                _authenticatedUser = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Logout: {ex.Message}");
            }
        }
        #endregion

        #endregion

        #region User Management Methods

        #region GetAuthenticatedUser
        // Retrieves the authenticated user.
        public User GetAuthenticatedUser()
        {
            try
            {
                if (_authenticatedUser == null)
                {
                    throw new InvalidOperationException("No user is currently authenticated.");
                }

                return _authenticatedUser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAuthenticatedUser: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region IsUserAuthenticated
        // Checks if a user is authenticated.
        public bool IsUserAuthenticated()
        {
            try
            {
                return _authenticatedUser != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in IsUserAuthenticated: {ex.Message}");
                return false;
            }
        }
        #endregion

        #endregion
    }
}
