using ExpenwiseTracker.Model;

namespace ExpenwiseTracker.Services.Interface
{
    public interface IUserService
    {
        bool AuthenticateUser(User user);
    }
}
