using Assignment.API.Models;
using Assignment.API.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

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

        public async Task<IEnumerable<Transaction>> GetByCurrencyAsync(string currency)
        {
            return await _transactionRepository.GetByCurrencyAsync(currency);
        }

        public async Task<IEnumerable<Transaction>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _transactionRepository.GetByDateRangeAsync(start, end);
        }

        public async Task<IEnumerable<Transaction>> GetByStatusAsync(string status)
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
                ".csv" => ParseCsv(file),
                //".xml" => ParseXml(file),
                _ => null // Unknown file format
            };
        }

        private List<Transaction>? ParseCsv(IFormFile file)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",", // The delimiter used in the CSV file
                HasHeaderRecord = false, // The file does not have a header row
                MissingFieldFound = null, // Ignore missing fields
                BadDataFound = context =>
                {
                    // Handle bad data (e.g., log the error)
                    Console.WriteLine($"Bad data found: {context.RawRecord}");
                },
                TrimOptions = TrimOptions.Trim, // Trim whitespace from fields
                IgnoreBlankLines = true, // Ignore blank lines in the file
            };

            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, config);

            // Configure CSV mapping
            csv.Context.RegisterClassMap<CsvTransactionMap>();

            var transactions = new List<Transaction>();
            var isValid = true;

            try
            {
                var records = csv.GetRecords<CsvTransactionRecord>().ToList();

                foreach (var record in records)
                {
                    if (!ValidateCsvRecord(record))
                    {
                        isValid = false;
                        break;
                    }

                    transactions.Add(new Transaction
                    {
                        Id = record.TransactionId,
                        AccountNumber = record.AccountNumber,
                        Amount = decimal.Parse(record.Amount, CultureInfo.InvariantCulture),
                        CurrencyCode = record.CurrencyCode,
                        TransactionDate = DateTime.ParseExact(record.TransactionDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                        Status = record.Status
                    });
                }
            }
            catch
            {
                // Log the error (e.g., invalid CSV format)
                return null;
            }

            return isValid ? transactions : null;
        }

        private bool ValidateCsvRecord(CsvTransactionRecord record)
        {
            // Validate mandatory fields
            if (string.IsNullOrEmpty(record.TransactionId) ||
                string.IsNullOrEmpty(record.AccountNumber) ||
                string.IsNullOrEmpty(record.Amount) ||
                string.IsNullOrEmpty(record.CurrencyCode) ||
                string.IsNullOrEmpty(record.TransactionDate) ||
                string.IsNullOrEmpty(record.Status))
            {
                return false;
            }

            // Validate TransactionId length
            if (record.TransactionId.Length > 50)
                return false;

            // Validate AccountNumber length
            if (record.AccountNumber.Length > 30)
                return false;

            // Validate CurrencyCode (ISO 4217 format)
            if (record.CurrencyCode.Length != 3)
                return false;

            // Validate Status
            var validStatuses = new[] { "Approved", "Failed", "Finished" };
            if (!validStatuses.Contains(record.Status))
                return false;

            // Validate Amount (must be a valid decimal)
            if (!decimal.TryParse(record.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                return false;

            // Validate TransactionDate (must be in the correct format)
            if (!DateTime.TryParseExact(record.TransactionDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                return false;

            return true;
        }

    }
}
