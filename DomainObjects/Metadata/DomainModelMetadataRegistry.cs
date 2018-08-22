using System;
using System.Collections.Generic;

namespace DomainObjects.Metadata
{
    internal static class DomainModelMetadataRegistry
    {
        static readonly Dictionary<string, DomainModelMetadata> entityModels = new Dictionary<string, DomainModelMetadata>();
        static readonly Dictionary<Type, DomainModelMetadata> entityModelMap = new Dictionary<Type, DomainModelMetadata>();
        static readonly Dictionary<Type, DomainEntityMetadata> entityDescriptors = new Dictionary<Type, DomainEntityMetadata>();

        public static void RegisterModel(DomainModelMetadata domainModel)
        {
            if (entityModels.ContainsKey(domainModel.ModelName))
                throw new InvalidOperationException($"A model with the name {domainModel.ModelName} has already been registered. Please use a distinct model name for each domain model.");

            entityModels.Add(domainModel.ModelName, domainModel);

            foreach (var descriptor in domainModel.GetEntityDescriptors())
            {
                if (entityModelMap.TryGetValue(descriptor.EntityType, out var modelMap))
                {
                    if (modelMap != domainModel)
                        throw new InvalidOperationException("Cannot register an entity with two different domain models");
                }
                else
                    entityModelMap.Add(descriptor.EntityType, domainModel);

                if (!entityDescriptors.ContainsKey(descriptor.EntityType))
                    entityDescriptors.Add(descriptor.EntityType, descriptor);
            }
        }

        public static DomainEntityMetadata GetEntityDescriptor(Type type)
        {
            //Assert Entity

            if (entityDescriptors.TryGetValue(type, out var descriptor))
                return descriptor;

            throw new KeyNotFoundException($"Domain entity type {type.Name} not found in any built models or model has not been built");
        }
    }
}
