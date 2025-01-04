using ExpenwiseTracker.Model;
using ExpenwiseTracker.Services.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenwiseTracker.Services
{
    public class TagService : ITagService
    {
        private readonly DbConnectionService _dbConnectionService;

        public TagService(DbConnectionService dbConnectionService)
        {
            _dbConnectionService = dbConnectionService;
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            var dbConnection = _dbConnectionService.EstablishConnection();
            return await Task.Run(() => dbConnection.Table<Tag>().ToList());
        }

        public async Task AddTagAsync(Tag tag)
        {
            var dbConnection = _dbConnectionService.EstablishConnection();

            var existingTag = await Task.Run(() =>
                dbConnection.Table<Tag>().FirstOrDefault(t => t.Name == tag.Name)
            );

            if (existingTag != null)
            {
                return;
            }

            await Task.Run(() => dbConnection.Insert(tag));
        }
    }
}
