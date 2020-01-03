using System;
using Unity.Entities;

namespace EntitiesBT
{
    public struct TickDeltaTime : IComponentData
    {
        public TimeSpan Value;
    }
}
