using ExpenwiseTracker.Model;

namespace ExpenwiseTracker.Services.Interface
{
    public interface IUserService
    {
        bool UserAuthentication(User user);

        // Get the authenticated user
        User GetAuthenticatedUser();

        // Logout method
        void Logout();

        // Check if user is authenticated
        bool IsUserAuthenticated();
    }
}
