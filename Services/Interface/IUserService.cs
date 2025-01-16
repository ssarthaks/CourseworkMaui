using ExpenwiseTracker.Model;

namespace ExpenwiseTracker.Services.Interface
{
    public interface IUserService
    {
        bool UserAuthentication(User user);

        User GetAuthenticatedUser();

        void Logout();

        bool IsUserAuthenticated();
    }
}
