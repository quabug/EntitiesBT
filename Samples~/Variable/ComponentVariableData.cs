using Unity.Entities;

namespace EntitiesBT.Sample
{
    [GenerateAuthoringComponent]
    public struct ComponentVariableData : IComponentData
    {
        public float InputValue;
        public int OutputValue;
        public long LongValue;
    }
}
