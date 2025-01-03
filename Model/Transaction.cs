using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ExpenwiseTracker.Model
{
    public class Transaction
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Type { get; set; } // Debit, Credit, or Debt
        public string Name { get; set; }
        public double Amount { get; set; }
        public string Tag { get; set; }
        public string Notes { get; set; } // Optional
        public string Source { get; set; } // For debts only
        public Boolean IsPaid { get; set; } = false; // For debts only
        public DateTime? DueDate { get; set; } // For debts only
        public DateTime Date { get; set; } // No need for AutoIncrement here
    }
}
