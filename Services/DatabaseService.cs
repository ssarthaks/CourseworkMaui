using ExpenwiseTracker.Model;
using ExpenwiseTracker.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenwiseTracker.Services
{
    public class DatabaseService
    {
        private readonly SQLiteConnection _connection;

        public DatabaseService(string dbPath)
        {
            // Initialize SQLite connection
            _connection = new SQLiteConnection(dbPath);

            // Create table if it doesn't exist
            _connection.CreateTable<Transaction>();
        }

        public SQLiteConnection EstablishConnection()
        {
            return _connection;
        }

        public List<Transaction> RetrieveAllTransactions()
        {
            // Fetch all transactions and return them as a list
            return _connection.Table<Transaction>().ToList();
        }
    }
}
