using Assignment.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment.API.Data
{
    public class TransactionContext : DbContext
    {
        public TransactionContext(DbContextOptions<TransactionContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
    }
}
