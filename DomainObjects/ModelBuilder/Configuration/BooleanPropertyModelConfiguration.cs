using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class BooleanPropertyModelConfiguration : PropertyModelConfiguration
    {
        internal BooleanPropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }

        public new BooleanPropertyModelConfiguration IsRequired()
        {
            base.IsRequired();
            return this;
        }
    }
}
