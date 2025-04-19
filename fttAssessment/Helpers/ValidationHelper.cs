using fttAssessment.Models;

namespace fttAssessment.Helpers
{
    public class ValidationHelper
    {
        #region Request Validation
        public static bool ValidateTransactionRequest(TransactionRequest request, out string validationMessage)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.PartnerKey))
            {
                validationMessage = "partnerkey is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.PartnerRefNo))
            {
                validationMessage = "partnerrefno is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.PartnerPassword))
            {
                validationMessage = "partnerpassword is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.Timestamp))
            {
                validationMessage = "timestamp is required.";
                return false;
            }

            // Validate amounts and items
            if (request.TotalAmount <= 0)
            {
                validationMessage = "TotalAmount must be a positive value.";
                return false;
            }

            if (request.Items == null || request.Items.Count == 0)
            {
                validationMessage = "At least one item is required.";
                return false;
            }

            // Validate each item
            foreach (var item in request.Items)
            {
                if (item.Qty < 1 || item.Qty > 5)
                {
                    validationMessage = "Item quantity must be between 1 and 5.";
                    return false;
                }

                if (item.UnitPrice <= 0)
                {
                    validationMessage = "UnitPrice must be a positive value.";
                    return false;
                }
            }

            validationMessage = string.Empty;
            return true;
        }

        public static bool ValidateTimestamp(string timestamp, out string message)
        {
            message = string.Empty;

            if (!DateTime.TryParse(timestamp, out var requestTime))
            {
                message = "Invalid timestamp format.";
                return false;
            }

            var serverTime = DateTime.UtcNow;
            var timeDifference = (requestTime - serverTime).Duration();

            if (timeDifference > TimeSpan.FromMinutes(5))
            {
                message = "Expired.";
                return false;
            }

            return true;
        }

        public static bool ValidateItemsTotal(TransactionRequest request, out string message)
        {
            message = string.Empty;

            if (request.Items == null || request.Items.Count == 0)
            {
                return true; // Already validated in main validation
            }

            long calculatedTotal = 0;
            foreach (var item in request.Items)
            {
                calculatedTotal += item.UnitPrice * item.Qty;
            }

            if (calculatedTotal != request.TotalAmount)
            {
                message = "Invalid Total Amount.";
                return false;
            }

            return true;
        }
        #endregion

        #region Response Validation
        public static bool ValidateTransactionResponse(TransactionResponse response, out string validationMessage)
        {
            // Validate Result field
            if (response.Result != 1 && response.Result != 0)
            {
                validationMessage = "Result must be either 1 (success) or 0 (error).";
                return false;
            }

            // Success case validation
            if (response.Result == 1)
            {
                if (response.TotalAmount.HasValue && response.TotalAmount <= 0)
                {
                    validationMessage = "TotalAmount must be a positive value when provided.";
                    return false;
                }

                if (response.TotalDiscount.HasValue && response.TotalDiscount <= 0)
                {
                    validationMessage = "TotalDiscount must be a positive value when provided.";
                    return false;
                }

                if (response.FinalAmount.HasValue && response.FinalAmount <= 0)
                {
                    validationMessage = "FinalAmount must be a positive value when provided.";
                    return false;
                }
            }
            else // Failure case validation
            {
                if (string.IsNullOrWhiteSpace(response.ResultMessage))
                {
                    validationMessage = "ResultMessage is required when operation fails.";
                    return false;
                }
            }

            validationMessage = "Validation successful.";
            return true;
        }
        #endregion

    }
}
