using Assignment.API.Data;
using Assignment.API.DTOs;
using Assignment.API.Models;
using Assignment.API.Utils;
using Microsoft.EntityFrameworkCore;

namespace Assignment.API.Repositories
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<TransactionDTO>> GetByCurrencyAsync(string currency);
        Task<IEnumerable<TransactionDTO>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<TransactionDTO>> GetByStatusAsync(string status);
    }

    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(TransactionContext context) : base(context) { }

        public async Task<IEnumerable<TransactionDTO>> GetByCurrencyAsync(string currency)
        {
            var query =
                from transaction in _context.Transactions
                where transaction.CurrencyCode == currency
                select new TransactionDTO { Id = transaction.Id, Payment = string.Concat(transaction.Amount.ToString("0.00"), " ", transaction.CurrencyCode), Status = ConvertUtils.StatusMapping(transaction.Status) };
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TransactionDTO>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            var query =
                from t in _context.Transactions
                where t.TransactionDate >= DateTime.SpecifyKind(start, DateTimeKind.Utc) && t.TransactionDate <= DateTime.SpecifyKind(end, DateTimeKind.Utc)
                select new TransactionDTO { Id = t.Id, Payment = string.Concat(t.Amount.ToString("0.00"), " ", t.CurrencyCode), Status = ConvertUtils.StatusMapping(t.Status) };
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TransactionDTO>> GetByStatusAsync(string status)
        {
            var query =
                from transaction in _context.Transactions
                where transaction.Status == status
                select new TransactionDTO { Id = transaction.Id, Payment = string.Concat(transaction.Amount.ToString("0.00"), " ", transaction.CurrencyCode), Status = ConvertUtils.StatusMapping(transaction.Status) };
            return await query.ToListAsync();
        }
    }
}
