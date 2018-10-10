using DomainObjects.Core;
using DomainObjects.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainObjects.Metadata
{
    internal static class DomainModelMetadataRegistry
    {
        static readonly Dictionary<string, DomainModelMetadata> entityModels = new Dictionary<string, DomainModelMetadata>();
        static readonly Dictionary<Type, DomainModelMetadata> entityModelMap = new Dictionary<Type, DomainModelMetadata>();
        static readonly Dictionary<Type, DomainEntityMetadata> entityDescriptors = new Dictionary<Type, DomainEntityMetadata>();
        static readonly Dictionary<Type, DomainValueObjectMetadata> valueObjectDescriptors = new Dictionary<Type, DomainValueObjectMetadata>();

        public static void RegisterModel(DomainModelMetadata domainModel)
        {
            if (entityModels.ContainsKey(domainModel.ModelName))
                throw new InvalidOperationException($"A model with the name {domainModel.ModelName} has already been registered. Please use a distinct model name for each domain model.");

            entityModels.Add(domainModel.ModelName, domainModel);

            foreach (var descriptor in domainModel.GetEntitiesMetadata())
            {
                if (entityModelMap.TryGetValue(descriptor.Type, out var modelMap))
                {
                    if (modelMap != domainModel)
                        throw new InvalidOperationException("Cannot register an entity with two different domain models");
                }
                else
                    entityModelMap.Add(descriptor.Type, domainModel);

                if (!entityDescriptors.ContainsKey(descriptor.Type))
                    entityDescriptors.Add(descriptor.Type, descriptor);
            }

            foreach (var descriptor in domainModel.GetValueObjectsMetadata())
            {
                if (entityModelMap.TryGetValue(descriptor.Type, out var modelMap))
                {
                    if (modelMap != domainModel)
                        throw new InvalidOperationException("Cannot register a value object with two different domain models");
                }
                else
                    entityModelMap.Add(descriptor.Type, domainModel);

                if (!valueObjectDescriptors.ContainsKey(descriptor.Type))
                    valueObjectDescriptors.Add(descriptor.Type, descriptor);
            }
        }

        public static DomainEntityMetadata GetEntityMetadta(Type type)
        {
            if (type.GetInterfaces().Contains(typeof(IDynamicProxy)))
                type = type.BaseType;

            if (!type.IsOrSubclassOfGenericDeep(typeof(Aggregate<,,>)) && !type.IsOrSubclassOfGenericDeep(typeof(AggregateRoot<,>)))
                throw new InvalidOperationException($"Given type {type.Name} is not a Domain Aggregate<T,TParent,TKey> or AggregateRoot<T,TKey>");

            if (entityDescriptors.TryGetValue(type, out var descriptor))
                return descriptor;

            throw new KeyNotFoundException($"Domain entity type {type.Name} not found in any built models or model has not been built");
        }

        public static DomainEntityMetadata GetEntityMetadta<T>() where T : DomainObject
            => GetEntityMetadta(typeof(T));

        public static DomainValueObjectMetadata GetValueObjectMetadata(Type type)
        {
            if (!type.IsOrSubclassOfGenericDeep(typeof(DomainValueObject<>)))
                throw new InvalidOperationException($"Given type {type.Name} is not a DomainValueObject<T>");

            if (valueObjectDescriptors.TryGetValue(type, out var descriptor))
                return descriptor;

            throw new KeyNotFoundException($"Domain value object type {type.Name} not found in any built models or model has not been built");
        }

        public static DomainValueObjectMetadata GetValueObjectMetadata<T>() where T : DomainValueObject<T>
            => GetValueObjectMetadata(typeof(T));
    }
}
