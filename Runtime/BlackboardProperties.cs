using Unity.Entities;

namespace EntitiesBT
{
    public struct BehaviorTreeTickDeltaTime : IComponentData
    {
        public float Value;
    }
    
    public struct ForceRunOnMainThreadTag : IComponentData {}
    public struct ForceRunOnJobTag : IComponentData {}
    public struct RunOnMainThreadTag : IComponentData {}
    // public struct RunOnJobTag : IComponentData {}

    public struct IsRunOnMainThread : IComponentData
    {
        public bool Value;
    }
}
