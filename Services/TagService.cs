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
            InitializeDefaultTags();
        }

        public async Task<List<Tag>> GetAllTags()
        {
            var dbConnection = _dbConnectionService.EstablishConnection();
            return await Task.Run(() => dbConnection.Table<Tag>().ToList());
        }

        public async Task AddTag(Tag tag)
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

        private void InitializeDefaultTags()
        {
            var dbConnection = _dbConnectionService.EstablishConnection();

            var defaultTags = new List<string>
        {
            "Yearly", "Monthly", "Food", "Drinks", "Clothes",
            "Gadgets", "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
        };

            foreach (var tagName in defaultTags)
            {
                var existingTag = dbConnection.Table<Tag>().FirstOrDefault(t => t.Name == tagName);
                if (existingTag == null)
                {
                    dbConnection.Insert(new Tag { Name = tagName });
                }
            }
        }
    }

}
