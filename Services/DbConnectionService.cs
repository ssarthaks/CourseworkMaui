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
    public class DbConnectionService
    {
        private readonly SQLiteConnection _connection;

        public DbConnectionService(string dbPath)

        {
            // Initialize SQLite connection
            _connection = new SQLiteConnection(dbPath);

            // Create table if it doesn't exist
            _connection.CreateTable<Transaction>();
            _connection.CreateTable<Tag>();
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

        public List<Tag> RetrieveAllTags()
        {
            // Fetch all tags and return them as a list
            return _connection.Table<Tag>().ToList();
        }

        public void AddTag(Tag tag)
        {
            // Check if the tag already exists
            var existingTag = _connection.Table<Tag>().FirstOrDefault(t => t.Name.Equals(tag.Name, StringComparison.OrdinalIgnoreCase));

            if (existingTag == null)
            {
                // Insert the new tag
                _connection.Insert(tag);
            }
        }
    }
}