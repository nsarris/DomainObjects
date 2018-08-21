using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class StringPropertyModelConfiguration : PropertyModelConfiguration
    {
        int? maxLength = 0;
        internal StringPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public StringPropertyModelConfiguration HasMaxLength(int maxLength)
        {
            this.maxLength = maxLength;
            return this;
        }

        public new StringPropertyModelConfiguration IsRequired()
        {
            base.IsRequired();
            return this;
        }
    }
}
