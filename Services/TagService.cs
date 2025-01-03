using ExpenwiseTracker.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenwiseTracker.Services
{
    public class TagService
    {
        private readonly DbConnectionService _dbConnectionService;

        public TagService(DbConnectionService dbConnectionService)
        {
            _dbConnectionService = dbConnectionService;
        }

        // Fetch all tags from the SQLite database asynchronously
        public async Task<List<Tag>> GetAllTagsAsync()
        {
            var dbConnection = _dbConnectionService.EstablishConnection();
            return await Task.Run(() => dbConnection.Table<Tag>().ToList());
        }

        // Add a new tag to the SQLite database asynchronously
        public async Task AddTagAsync(Tag tag)
        {
            var dbConnection = _dbConnectionService.EstablishConnection();

            // Check if the tag already exists
            var existingTag = await Task.Run(() =>
                dbConnection.Table<Tag>().FirstOrDefault(t => t.Name == tag.Name)
            );

            if (existingTag != null)
            {
                // Tag already exists, handle accordingly (e.g., notify user or return)
                return;
            }

            // Add the new tag if it doesn't exist
            await Task.Run(() => dbConnection.Insert(tag)); // Assuming Insert is an async operation
        }
    }
}