using System;

namespace EntitiesBT.Core
{
    [Flags]
    public enum NodeState
    {
        None = 0,
        Success = 1 << 0,
        Failure = 1 << 1,
        Running = 1 << 2
    }
}
