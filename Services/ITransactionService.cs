using Assignment.API.Models;

namespace Assignment.API.Services
{
    public interface ITransactionService
    {
        Task UploadFileAsync(IFormFile file);
        Task<IEnumerable<Transaction>> GetByCurrencyAsync(string currency);
        Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<Transaction>> GetByStatusAsync(string status);
    }
}
