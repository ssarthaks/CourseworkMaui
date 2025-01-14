using ExpenwiseTracker.Model;
using SQLite;
using System.Collections.Generic;

namespace ExpenwiseTracker.Services
{
    public class DbConnectionService
    {
        private readonly SQLiteConnection _connection;

        #region Constructor
        public DbConnectionService(string dbPath)
        {
            _connection = new SQLiteConnection(dbPath);

            _connection.CreateTable<Transaction>();
            _connection.CreateTable<Tag>();
        }
        #endregion

        #region Connection Methods
        public SQLiteConnection EstablishConnection()
        {
            return _connection;
        }
        #endregion

        #region Transaction Data Retrieval
        public List<Transaction> RetrieveAllTransactions()
        {
            return _connection.Table<Transaction>().ToList();
        }
        #endregion
    }
}
