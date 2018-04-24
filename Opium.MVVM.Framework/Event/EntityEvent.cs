using System;
using Opium.MVVM.Framework.Properties;

namespace Opium.MVVM.Framework.Event
{
    public class EntityEvent
    {
      
        public static EntityEvent CreateArgsFor(object entity, EntityCommand command)
        {
            return new EntityEvent
            {
                Command = command,
                Entity = entity,
                EntityType = entity.GetType()
            };
        }
        
        public static EntityEvent CreateArgsFor<T>(object entity, EntityCommand command)
        {
            return new EntityEvent
            {
                Command = command,
                Entity = entity,
                EntityType = typeof(T)
            };
        }
       
        public Type EntityType { get; private set; }

        public EntityCommand Command { get; private set; }

       
        public object Entity { get; private set; }

        public T EntityForType<T>()
        {
            return Entity == null ? default(T) : (T)Entity;
        }

        public override string ToString()
        {
            return string.Format(Resources.EntityEvent_ToString_EntityEvent, Command, EntityType.FullName, Entity);
        }
    }
}
