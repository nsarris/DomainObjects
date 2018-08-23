using System;

namespace DomainObjects.Core
{
    public struct DomainNumber //: DomainValue
    {
        //event
        decimal? number;
        
        public bool IsSet { get; private set; }
        public bool IsNull => !number.HasValue;
        public bool HasValue => IsSet && !IsNull;

        public DomainNumber(decimal number)
            : this()
        {
            this.number = number;
            IsSet = true;
        }

        public void UnSet()
        {
            number = default(decimal);
            IsSet = false;
        }

        public void SetNull()
        {
            number = null;
            IsSet = true;
        }

        public decimal Value
        {
            get
            {
                return number ?? 0;
            }
            set
            {
                number = value;
                IsSet = true;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is DomainNumber other)
            {
                if (this.HasValue != other.HasValue)
                    return false;

                return this.Value == other.Value;
            }
            else
                return false;
        }

        #pragma warning disable S2328
        public override int GetHashCode()
        {
            if (!HasValue)
                return 0;
            else
                return number.GetHashCode();
        }
        #pragma warning restore S2328

        //implicit conversions
    }
}
