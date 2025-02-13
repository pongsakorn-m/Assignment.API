using Assignment.API.DTOs;
using Assignment.API.Models;
using Assignment.API.Repositories;
using Assignment.API.Utils;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Xml.Linq;

namespace Assignment.API.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            var transactions = ParseFile(file);
            if (transactions == null || transactions.Count == 0)
                throw new ArgumentException("Invalid file content.");

            foreach (var transaction in transactions)
            {
                await _transactionRepository.AddAsync(transaction);
            }
        }

        public async Task<IEnumerable<TransactionDTO>> GetByCurrencyAsync(string currency)
        {
            return (await _transactionRepository.GetByCurrencyAsync(currency));
        }

        public async Task<IEnumerable<TransactionDTO>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _transactionRepository.GetByDateRangeAsync(start, end);
        }

        public async Task<IEnumerable<TransactionDTO>> GetByStatusAsync(string status)
        {
            return await _transactionRepository.GetByStatusAsync(status);
        }

        private List<Transaction>? ParseFile(IFormFile file)
        {
            // Implement file parsing logic (CSV/XML)
            // Validate records and return a list of valid transactions
            // If any record is invalid, return null
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            return fileExtension switch
            {
                ".csv" => ConvertUtils.ParseCsv(file),
                ".xml" => ConvertUtils.ParseXml(file),
                _ => null // Unknown file format
            };
        }

    }
}
