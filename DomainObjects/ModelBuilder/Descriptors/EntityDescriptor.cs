using DomainObjects.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.ModelBuilder.Descriptors
{
    internal class EntityDescriptor
    {
        public Type Type { get; }
        public bool IsRoot { get; }
        public List<PropertyDescriptor> PropertyDescriptors { get; }
        public List<EntityDescriptor> AggregateDescriptors { get; }
        public List<ValueTypeDescriptor> ValueTypeDescriptors { get; }
        public EntityDescriptor(Type type, List<PropertyDescriptor> propertyDescriptors, List<EntityDescriptor> aggregateDescriptors, List<ValueTypeDescriptor> valueTypeDescriptors)
        {
            Type = type;
            IsRoot = type.IsOrSubclassOfGenericDeep(typeof(AggregateRoot<,>));
            AggregateDescriptors = aggregateDescriptors;
            ValueTypeDescriptors = valueTypeDescriptors;
            PropertyDescriptors = propertyDescriptors.ToList();
        }
    }
}
