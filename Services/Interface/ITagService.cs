using ExpenwiseTracker.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenwiseTracker.Services.Interface
{
    public interface ITagService
    {
        Task<List<Tag>> GetAllTagsAsync();

        Task AddTagAsync(Tag tag);
    }
}
