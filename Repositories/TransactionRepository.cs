using Assignment.API.Data;
using Assignment.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment.API.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByCurrencyAsync(string currency);
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<Transaction>> GetByStatusAsync(string status);
    }

    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(TransactionContext context) : base(context) { }

        public async Task<IEnumerable<Transaction>> GetByCurrencyAsync(string currency)
        {
            return await _context.Transactions.Where(t => t.CurrencyCode == currency).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Transactions.Where(t => t.TransactionDate >= DateTime.SpecifyKind(start, DateTimeKind.Utc) && t.TransactionDate <= DateTime.SpecifyKind(end, DateTimeKind.Utc)).ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByStatusAsync(string status)
        {
            return await _context.Transactions.Where(t => t.Status == status).ToListAsync();
        }
    }
}
