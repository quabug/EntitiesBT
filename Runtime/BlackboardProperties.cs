using Unity.Entities;

namespace EntitiesBT
{
    public struct TickDeltaTime : IComponentData
    {
        public float Value;
    }
    
    public struct IsMainThread : IComponentData
    {
        public bool Value;
    }
}
