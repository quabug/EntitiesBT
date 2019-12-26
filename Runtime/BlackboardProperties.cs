using System;

namespace EntitiesBT
{
    public readonly struct TickDeltaTime
    {
        public readonly TimeSpan Value;
        
        public TickDeltaTime(TimeSpan value)
        {
            Value = value;
        }
    }
}
