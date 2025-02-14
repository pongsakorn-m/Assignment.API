using Assignment.API.Models;

namespace Assignment.API.Services
{
    public interface IConvertService
    {
        public List<Transaction>? ParseCsv(IFormFile file);
        public List<Transaction>? ParseXml(IFormFile file);
    }
}
