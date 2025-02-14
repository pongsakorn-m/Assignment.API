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
        private readonly IConvertService _convertService;

        public TransactionService(ITransactionRepository transactionRepository, IConvertService convertService)
        {
            _transactionRepository = transactionRepository;
            _convertService = convertService;
        }

        public async Task UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException(nameof(file));
            }

            var errorMessage = new List<string>();

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var fileExtensionAllow = new List<string>() { ".xml", ".csv" };
            if (!fileExtensionAllow.Contains(fileExtension))
            {
                errorMessage.Add("File extension is invalid.");
            }

            // for setup max file size to 1 MB.
            var maxFileSize = 1 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                errorMessage.Add("File size must be less than 1 MB");
            }

            if (errorMessage.Count > 0)
            {
                throw new FormatException(string.Join(", ", errorMessage));
            }

            var transactions = ParseFile(file);
            if (transactions == null || transactions.Count == 0)
            {
                throw new FormatException("Invalid file content.");
            }

            foreach (var transaction in transactions)
            {
                await _transactionRepository.AddAsync(transaction);
            }
        }

        public async Task<IEnumerable<TransactionDTO>> GetByCurrencyAsync(string currency)
        {
            return await _transactionRepository.GetByCurrencyAsync(currency);
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
                ".csv" => _convertService.ParseCsv(file),
                ".xml" => _convertService.ParseXml(file),
                _ => null // Unknown file format
            };
        }

    }
}
