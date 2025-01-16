using ExpenwiseTracker.Model;
using SQLite;

namespace ExpenwiseTracker.Services
{
    public class DbConnectionService
    {
        private readonly SQLiteConnection _connection;

        #region Constructor
        // Initializes the database connection and creates necessary tables.
        public DbConnectionService(string dbPath)
        {
            try
            {
                if (string.IsNullOrEmpty(dbPath))
                {
                    throw new ArgumentNullException(nameof(dbPath), "Database path cannot be null or empty.");
                }

                _connection = new SQLiteConnection(dbPath);

                // Create necessary tables
                _connection.CreateTable<Transaction>();
                _connection.CreateTable<Tag>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database connection: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Connection Methods
        // Establishes and returns the SQLite connection.
        public SQLiteConnection EstablishConnection()
        {
            try
            {
                if (_connection == null)
                {
                    throw new InvalidOperationException("Database connection is not initialized.");
                }

                return _connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error establishing database connection: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Transaction Data Retrieval
        // Retrieves all transactions from the database.
        public List<Transaction> RetrieveAllTransactions()
        {
            try
            {
                return _connection.Table<Transaction>().ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving transactions: {ex.Message}");
                return new List<Transaction>();
            }
        }
        #endregion
    }
}
