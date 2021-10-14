using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Demo.Models
{
    public class DemoDB : DbContext
    {
        //All tables in database entered here
        public DemoDB() : base("DemoConnection")
        {

        }

        public DbSet<DebtorMaster> DebtorMasters { get; set; }
        public DbSet<StockMaster> stockMasters { get; set; }

        public DbSet<DebtorsTransaction> debtorsTransactions { get; set; }

        public DbSet<StockTransactionFile> stockTransactionFiles { get; set; }

        public DbSet<InvoiceDebt> invoiceDebts { get; set; }
        public DbSet<User> Users { get; set; }
    }
}