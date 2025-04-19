using fttAssessment.Helpers;
using fttAssessment.Models;
using fttAssessment.Services;
using Microsoft.AspNetCore.Mvc;
using log4net;
using System.Text.Json;

namespace fttAssessment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private static readonly ILog _logger = LogManager.GetLogger(typeof(TransactionController));

        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost("submittrxmessage")]
        public async Task<IActionResult> SubmitTransactionAsync([FromBody] TransactionRequest request)
        {
            try
            {
                _logger.Info($"Received transaction request: {JsonSerializer.Serialize(request)}");

                //Check request body
                if (!ValidationHelper.ValidateTransactionRequest(request, out var validationMessage))
                {
                    return BadRequest(new TransactionResponse
                    {
                        Result = 0,
                        ResultMessage = validationMessage
                    });
                }

                //Check timestamp expiration (±5 minutes)
                if (!ValidationHelper.ValidateTimestamp(request.Timestamp, out var timestampMessage))
                {
                    return BadRequest(new TransactionResponse
                    {
                        Result = 0,
                        ResultMessage = timestampMessage
                    });
                }

                //Validate items total matches totalAmount
                if (!ValidationHelper.ValidateItemsTotal(request, out var totalAmountMessage))
                {
                    return BadRequest(new TransactionResponse
                    {
                        Result = 0,
                        ResultMessage = totalAmountMessage
                    });
                }

                //Verify signature
                if (!SignatureHelper.VerifySignature(request))
                {
                    return Unauthorized(new TransactionResponse
                    {
                        Result = 0,
                        ResultMessage = "Access Denied!"
                    });
                }

                //Calculate discounts and final amount
                var discountResult = await _transactionService.CalculateDiscounts(request.TotalAmount);


                var response = new TransactionResponse
                {
                    Result = 1,
                    TotalAmount = request.TotalAmount,
                    TotalDiscount = discountResult.totalDiscount,
                    FinalAmount = discountResult.finalAmount
                };

                _logger.Info($"Returning successful response: {JsonSerializer.Serialize(response)}");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Transaction processing error: {ex.Message}", ex);
                return StatusCode(500, new TransactionResponse
                {
                    Result = 0,
                    ResultMessage = "An internal error occurred"
                });
            }
        }
    }
}