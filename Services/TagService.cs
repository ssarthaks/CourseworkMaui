using ExpenwiseTracker.Model;
using ExpenwiseTracker.Services.Interface;

namespace ExpenwiseTracker.Services
{
    public class TagService : ITagService
    {
        private readonly DbConnectionService _dbConnectionService;

        #region Constructor
        // Initializes the TagService with a DbConnectionService and also initializes default tags.
        public TagService(DbConnectionService dbConnectionService)
        {
            try
            {
                _dbConnectionService = dbConnectionService ?? throw new ArgumentNullException(nameof(dbConnectionService), "Database connection service cannot be null.");
                InitializeDefaultTags();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during TagService initialization: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Get Tags and Add Tag

        // Retrieves all tags from the database asynchronously.
        public async Task<List<Tag>> GetAllTags()
        {
            try
            {
                var dbConnection = _dbConnectionService.EstablishConnection();
                return await Task.Run(() => dbConnection.Table<Tag>().ToList());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving tags: {ex.Message}");
                return new List<Tag>();
            }
        }

        // Adds a new tag to the database if it does not already exist.
        public async Task AddTag(Tag tag)
        {
            try
            {
                if (tag == null)
                {
                    throw new ArgumentNullException(nameof(tag), "Tag cannot be null.");
                }

                var dbConnection = _dbConnectionService.EstablishConnection();

                var existingTag = await Task.Run(() =>
                    dbConnection.Table<Tag>().FirstOrDefault(t => t.Name == tag.Name)
                );

                if (existingTag == null)
                {
                    await Task.Run(() => dbConnection.Insert(tag));
                }
                else
                {
                    Console.WriteLine($"Tag '{tag.Name}' already exists.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding tag: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region Initialize Default Tags
        // Initializes default tags in the database if they do not already exist.
        private void InitializeDefaultTags()
        {
            try
            {
                var dbConnection = _dbConnectionService.EstablishConnection();

                var defaultTags = new List<string>
                {
                    "Yearly", "Monthly", "Food", "Drinks", "Clothes",
                    "Gadgets", "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
                };

                foreach (var tagName in defaultTags)
                {
                    try
                    {
                        var existingTag = dbConnection.Table<Tag>().FirstOrDefault(t => t.Name == tagName);
                        if (existingTag == null)
                        {
                            dbConnection.Insert(new Tag { Name = tagName });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error initializing default tag '{tagName}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing default tags: {ex.Message}");
            }
        }
        #endregion
    }
}
