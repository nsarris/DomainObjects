using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.ModelBuilder.Descriptors
{
    internal class ValueTypeDescriptor
    {
        public Type Type { get; }
        public List<PropertyDescriptor> PropertyDescriptors { get; }
        public List<ValueTypeDescriptor> ValueTypeDescriptors { get; }
        public ValueTypeDescriptor(Type type, List<PropertyDescriptor> propertyDescriptors, List<ValueTypeDescriptor> valueTypeDescriptors)
        {
            Type = type;
            ValueTypeDescriptors = valueTypeDescriptors;
            PropertyDescriptors = propertyDescriptors.ToList();
        }
    }
}
