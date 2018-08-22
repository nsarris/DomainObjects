using System.Reflection;

namespace DomainObjects.ModelBuilder.Configuration
{
    public class UnsupportedTypePropertyModelConfiguration : PropertyModelConfiguration
    {
        public UnsupportedTypePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
