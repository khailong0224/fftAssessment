using fttAssessment.Models;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace fttAssessment.Helpers
{
    public static class SignatureHelper
    {
        public static bool VerifySignature(TransactionRequest request)
        {
            // Step 1: Parse and format the timestamp
            if (!DateTime.TryParse(request.Timestamp, out var parsedTimestamp))
            {
                return false; // Invalid timestamp format
            }

            // Use UTC and strict formatting
            string formattedTimestamp = parsedTimestamp.ToUniversalTime().ToString("yyyyMMddHHmmss");

            // Step 2: Construct the string in strict order
            string concatenatedParams = formattedTimestamp +
                                      request.PartnerKey +
                                      request.PartnerRefNo +
                                      request.TotalAmount.ToString(CultureInfo.InvariantCulture) +
                                      request.PartnerPassword; // Using Base64 encoded password directly

            // Step 3: SHA256 hash -> hex string -> Base64
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedParams));

                // Convert to hex string
                string hexString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                // Convert hex string to Base64
                byte[] hexAsBytes = Encoding.ASCII.GetBytes(hexString);
                string computedSig = Convert.ToBase64String(hexAsBytes);

                return computedSig == request.Sig;
            }
        }
    }
}
