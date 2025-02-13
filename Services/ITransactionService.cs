using Assignment.API.DTOs;
using Assignment.API.Models;

namespace Assignment.API.Services
{
    public interface ITransactionService
    {
        Task UploadFileAsync(IFormFile file);
        Task<IEnumerable<TransactionDTO>> GetByCurrencyAsync(string currency);
        Task<IEnumerable<TransactionDTO>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<IEnumerable<TransactionDTO>> GetByStatusAsync(string status);
    }
}
