using ExpenwiseTracker.Model;

namespace ExpenwiseTracker.Services.Interface
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllTags();
        Task AddTag(Tag tag);
    }
}
