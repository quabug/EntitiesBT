using System.Runtime.InteropServices;
using Unity.Entities;

namespace EntitiesBT
{
    public struct BehaviorTreeTickDeltaTime : IComponentData
    {
        public float Value;
    }
    
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct ForceRunOnMainThreadTag : IComponentData {}
    
    [StructLayout(LayoutKind.Explicit, Size = 1)]
    public struct RunOnMainThreadTag : IComponentData {}

    public struct IsRunOnMainThread : IComponentData
    {
        public bool Value;
    }
}
