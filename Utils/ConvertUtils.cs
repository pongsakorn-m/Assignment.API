﻿using Assignment.API.Models;
using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;
using System.Xml.Linq;

namespace Assignment.API.Utils
{
    public class ConvertUtils
    {
        public static string StatusMapping(string status)
        {
            if (string.Equals(status, "Approved", StringComparison.OrdinalIgnoreCase))
            {
                return "A";
            }
            else if (string.Equals(status, "Failed", StringComparison.OrdinalIgnoreCase) || string.Equals(status, "Rejected", StringComparison.OrdinalIgnoreCase))
            {
                return "R";
            }
            else if (string.Equals(status, "Finished", StringComparison.OrdinalIgnoreCase) || string.Equals(status, "Done", StringComparison.OrdinalIgnoreCase))
            {
                return "D";
            }
            else
            {
                return "";
            }
        }
        public static List<Transaction>? ParseCsv(IFormFile file)
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

        private static bool ValidateCsvRecord(CsvTransactionRecord record)
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

            if (record.CurrencyCode.Length != 3)
                return false;

            if (!decimal.TryParse(record.Amount, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                return false;

            if (!DateTime.TryParseExact(record.TransactionDate, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                return false;

            return true;
        }


        public static List<Transaction>? ParseXml(IFormFile file)
        {
            try
            {
                using var stream = file.OpenReadStream();
                var xmlDoc = XDocument.Load(stream);

                var transactions = new List<Transaction>();
                var isValid = true;

                foreach (var element in xmlDoc.Descendants("Transaction"))
                {
                    var transaction = new Transaction
                    {
                        Id = element.Attribute("id")?.Value,
                        AccountNumber = element.Element("PaymentDetails")?.Element("AccountNo")?.Value,
                        Amount = decimal.Parse(element.Element("PaymentDetails")?.Element("Amount")?.Value, CultureInfo.InvariantCulture),
                        CurrencyCode = element.Element("PaymentDetails")?.Element("CurrencyCode")?.Value,
                        TransactionDate = DateTime.Parse(element.Element("TransactionDate")?.Value),
                        Status = element.Element("Status")?.Value
                    };

                    if (!ValidateXmlTransaction(transaction))
                    {
                        isValid = false;
                        break;
                    }

                    transactions.Add(transaction);
                }

                return isValid ? transactions : null;
            }
            catch
            {
                // Log the error (e.g., invalid XML format)
                return null;
            }
        }

        private static bool ValidateXmlTransaction(Transaction transaction)
        {
            // Validate mandatory fields
            if (string.IsNullOrEmpty(transaction.Id) ||
                string.IsNullOrEmpty(transaction.AccountNumber) ||
                string.IsNullOrEmpty(transaction.CurrencyCode) ||
                string.IsNullOrEmpty(transaction.Status))
            {
                return false;
            }

            if (transaction.Id.Length > 50)
                return false;

            if (transaction.AccountNumber.Length > 30)
                return false;

            if (transaction.CurrencyCode.Length != 3)
                return false;

            return true;
        }

    }
}
