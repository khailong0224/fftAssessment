using System;
using System.Threading.Tasks;
using fttAssessment.Models;

namespace fttAssessment.Services
{
    public interface ITransactionService
    {
        Task<(long totalDiscount, long finalAmount)> CalculateDiscounts(long totalAmountInCents);
    }
    public class TransactionService : ITransactionService
    {
        public async Task<(long totalDiscount, long finalAmount)> CalculateDiscounts(long totalAmountInCents)
        {
            decimal totalAmountMYR = totalAmountInCents / 100m;

            // Calculate base discount
            decimal baseDiscountPercent = GetBaseDiscountPercentage(totalAmountMYR);

            // Calculate conditional discounts
            decimal conditionalDiscountPercent = GetConditionalDiscountPercentage(totalAmountMYR);

            // Apply discount cap
            decimal totalDiscountPercent = Math.Min(
                baseDiscountPercent + conditionalDiscountPercent,
                20m); // Max 20%

            // Calculate amounts
            decimal totalDiscountMYR = totalAmountMYR * (totalDiscountPercent / 100m);
            long totalDiscountInCents = (long)Math.Round(totalDiscountMYR * 100);
            long finalAmountInCents = totalAmountInCents - totalDiscountInCents;

            return (totalDiscountInCents, finalAmountInCents);
        }

        private decimal GetBaseDiscountPercentage(decimal amountMYR)
        {
            return amountMYR switch
            {
                < 200m => 0m,
                >= 200m and <= 500m => 5m,
                >= 501m and <= 800m => 7m,
                >= 801m and <= 1200m => 10m,
                > 1200m => 15m,
                _ => 0m
            };
        }

        private decimal GetConditionalDiscountPercentage(decimal amountMYR)
        {
            decimal discount = 0m;

            // Prime number discount
            if (amountMYR > 500m && IsPrime((long)amountMYR))
            {
                discount += 8m;
            }

            // Ends with 5 discount
            if (amountMYR > 900m && amountMYR % 10 == 5)
            {
                discount += 10m;
            }

            return discount;
        }

        private bool IsPrime(long number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (long)Math.Floor(Math.Sqrt(number));

            for (long i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0) return false;
            }

            return true;
        }
    }
}