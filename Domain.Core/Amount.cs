using System.Collections.Generic;
using System.Linq;

namespace FreedomFridayServerless.Domain.Core
{
    public sealed class Amount: ValueObject<Amount>
    {       
        public static readonly Amount Zero = new Amount(0m, 0m);

        public static Amount Allocate(decimal debit, decimal credit)
        {
            return new Amount(debit, credit);
        }

        public static Amount Allocate(decimal netAmount)
        {
            return new Amount(netAmount);
        }

        public decimal Net { get; private set; }

        public decimal Debit { get; private set; }
        public decimal Credit { get; private set; }

        private Amount(decimal debit, decimal credit)
        {
            Debit = debit;
            Credit = credit;

            CalculateNet();
        }

        private Amount(decimal netAmount)
        {
            if (netAmount > 0)
                Debit = netAmount;
            else
                Credit = decimal.Negate(netAmount);

            Net = netAmount;
        }

        private void CalculateNet()
        {
            Net = Debit - Credit;
        }

        public static Amount operator +(Amount amount1, Amount amount2)
        {
            return new Amount(
                amount1.Debit + amount2.Debit,
                amount1.Credit + amount2.Credit);
        }

        public static Amount operator -(Amount amount1, Amount amount2)
        {
            return new Amount(
                amount1.Debit - amount2.Debit,
                amount1.Credit - amount2.Credit);
        }

        protected override bool EqualsCore(Amount other)
        {
            return Net == other.Net;
        }

        //check http://bit.ly/1FSzTg1 for explanation
        protected override int GetHashCodeCore()
        {
            unchecked
            {
                return Net.GetHashCode();
            }
        }
    }

    public static class AmountExtensions
    {
        public static Amount Sum(this IEnumerable<Amount> values)
        {
            var total = values.Aggregate(Amount.Zero, (current, amount) => current + amount);

            return total;
        }

        public static bool IsZero(this Amount amount)
        {
            return amount == Amount.Zero;
        }

        public static Amount Negate(this Amount amount)
        {
            return Amount.Allocate(decimal.Negate(amount.Debit), decimal.Negate(amount.Credit));
        }

        public static Amount Reverse(this Amount amount)
        {
            return Amount.Allocate(debit: amount.Credit, credit: amount.Debit);
        }
    }
}