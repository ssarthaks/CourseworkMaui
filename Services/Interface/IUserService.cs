using ExpenwiseTracker.Model;

namespace ExpenwiseTracker.Services.Interface
{
    public interface IUserService
    {
        bool AuthenticateUser(User user);
        User GetAuthenticatedUser(); // Get the authenticated user
        void Logout(); // Logout method
        bool IsUserAuthenticated(); // Check if user is authenticated
    }
}
