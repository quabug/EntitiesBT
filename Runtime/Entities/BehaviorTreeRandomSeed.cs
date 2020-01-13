using Unity.Entities;

namespace EntitiesBT.Entities
{
    [GenerateAuthoringComponent]
    public struct BehaviorTreeRandomSeed : IComponentData
    {
        public uint Value;
    }
}
