using System;

namespace DomainObjects.Core
{
    public class DomainKeyAttribute : Attribute
    {
        public int Index { get; private set; }
        public DomainKeyAttribute(int index)
        {
            this.Index = index;
        }
    }
}
