using System.Reflection;

namespace DomainObjects.ModelBuilder
{
    public class UnsupportedTypePropertyModelConfiguration : PropertyModelConfiguration
    {
        public UnsupportedTypePropertyModelConfiguration(PropertyInfo property) : base(property)
        {
        }
    }
}
