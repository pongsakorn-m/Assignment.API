using Assignment.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("Upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                await _transactionService.UploadFileAsync(file);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ByCurrency/{currency}")]
        public async Task<IActionResult> GetByCurrency(string currency)
        {
            var transactions = await _transactionService.GetByCurrencyAsync(currency);
            return Ok(transactions);
        }

        [HttpGet("ByDateRange/{start}/{end}")]
        public async Task<IActionResult> GetByDateRange(DateTime start, DateTime end)
        {
            var transactions = await _transactionService.GetByDateRangeAsync(start, end);
            return Ok(transactions);
        }

        [HttpGet("ByStatus/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            var transactions = await _transactionService.GetByStatusAsync(status);
            return Ok(transactions);
        }
    }
}
