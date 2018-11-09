using Dynamix.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Core
{
    internal class DomainEntityVisitor
    {
        public readonly static DomainEntityVisitor Instance = new DomainEntityVisitor();

        public void Visit(DomainEntity entity, Action<DomainEntity> action)
        {
            Visit(entity, action, new HashSet<object>());
        }

        private void Visit(DomainEntity entity, Action<DomainEntity> action, HashSet<object> visited)
        {
            if (entity == null || visited.Contains(entity))
                return;

            visited.Add(entity);

            action(entity);

            var metadata = entity.GetEntityMetadata();

            foreach (var entityProperty in metadata.GetAggregateProperties())
                Visit((DomainEntity)entityProperty.Property.Get(entity), action, visited);

            foreach (var entityListProperty in metadata.GetAggregateListProperties())
            {
                var entityList = (System.Collections.IEnumerable)entityListProperty.Property.Get(entity);
                foreach(var listItem in entityList.OfType<DomainEntity>())
                    Visit(listItem, action, visited);
            }
        }
    }
}

