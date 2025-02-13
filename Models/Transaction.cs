using CsvHelper.Configuration;
using System.Globalization;

namespace Assignment.API.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }

        private DateTime _transactionDate;
        public DateTime TransactionDate
        {
            get => _transactionDate;
            set => _transactionDate = DateTime.SpecifyKind(value, DateTimeKind.Utc); // Force UTC
        }

        public string Status { get; set; }
    }

    public class CsvTransactionRecord
    {
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string TransactionDate { get; set; }
        public string Status { get; set; }
    }

    // CSV mapping configuration
    public sealed class CsvTransactionMap : ClassMap<CsvTransactionRecord>
    {
        public CsvTransactionMap()
        {
            Map(m => m.TransactionId).Index(0);
            Map(m => m.AccountNumber).Index(1);
            Map(m => m.Amount).Index(2);
            Map(m => m.CurrencyCode).Index(3);
            Map(m => m.TransactionDate).Index(4);
            Map(m => m.Status).Index(5);
        }
    }
}
