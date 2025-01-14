using ExpenwiseTracker.Model;
using SQLite;
using System.Collections.Generic;

namespace ExpenwiseTracker.Services
{
    public class DbConnectionService
    {
        private readonly SQLiteConnection _connection;

        public DbConnectionService(string dbPath)
        {
            _connection = new SQLiteConnection(dbPath);

            _connection.CreateTable<Transaction>();
            _connection.CreateTable<Tag>();
        }

        public SQLiteConnection EstablishConnection()
        {
            return _connection;
        }

        public List<Transaction> RetrieveAllTransactions()
        {
            return _connection.Table<Transaction>().ToList();
        }
    }
}
