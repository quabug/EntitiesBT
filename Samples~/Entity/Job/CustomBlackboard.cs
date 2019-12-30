using System;
using EntitiesBT.Core;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace EntitiesBT.Sample
{
    public unsafe struct CustomBlackboard : IComponentData, IBlackboard
    {
        public TickDeltaTime TickDeltaTime;
        public Translation* Translation;
        
        public object this[object key]
        {
            get
            {
                if (key is Type type)
                {
                    if (type == typeof(TickDeltaTime)) return TickDeltaTime;
                    if (type == typeof(Translation)) return *Translation;
                }
                throw new NotImplementedException();
            }
            set
            {
                if (key is Type type)
                {
                    if (type == typeof(TickDeltaTime))
                    {
                        TickDeltaTime = (TickDeltaTime) value;
                        return;
                    }
                    if (type == typeof(Translation))
                    {
                        Translation->Value = ((Translation)value).Value;
                        return;
                    }
                }
                throw new NotImplementedException();
            }
        }
    }
}
