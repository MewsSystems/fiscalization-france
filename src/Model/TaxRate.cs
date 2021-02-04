using System;
using FuncSharp;

namespace Mews.Fiscalization.SignatureChecker.Model
{
    internal sealed class TaxRate : Product1<decimal>, IComparable<TaxRate>
    {
        public TaxRate(decimal value)
            : base(value)
        {
        }

        public decimal Value
        {
            get { return ProductValue1; }
        }

        public int CompareTo(TaxRate other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}