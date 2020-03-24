using System.Runtime.InteropServices;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ForceRunOnMainThreadTag : IComponentData {}
    
    [StructLayout(LayoutKind.Explicit)]
    public struct ForceRunOnJobTag : IComponentData {}
    
    [StructLayout(LayoutKind.Explicit)]
    public struct RunOnMainThreadTag : IComponentData {}
}
