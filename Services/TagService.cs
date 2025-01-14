using ExpenwiseTracker.Model;
using ExpenwiseTracker.Services.Interface;

namespace ExpenwiseTracker.Services
{
    public class TagService : ITagService
    {
        private readonly DbConnectionService _dbConnectionService;

        #region Constructor
        // Initializes the TagService with a DbConnectionService and also initialize default tag 
        public TagService(DbConnectionService dbConnectionService)
        {
            _dbConnectionService = dbConnectionService;
            InitializeDefaultTags();
        }
        #endregion

        #region Get Tags and Add Tag
        // Retrieves all tags from the database asynchronously.
        public async Task<List<Tag>> GetAllTags()
        {
            var dbConnection = _dbConnectionService.EstablishConnection();
            return await Task.Run(() => dbConnection.Table<Tag>().ToList());
        }

        // Adds a new tag to the database if it does not already exist.
        public async Task AddTag(Tag tag)
        {
            var dbConnection = _dbConnectionService.EstablishConnection();

            var existingTag = await Task.Run(() =>
                dbConnection.Table<Tag>().FirstOrDefault(t => t.Name == tag.Name)
            );

            if (existingTag == null)
            {
                await Task.Run(() => dbConnection.Insert(tag));
            }
        }

        #endregion

        #region Initialize Default Tags
        // Initializes default tags in the database if they do not already exist.
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
        #endregion
    }
}
