using Unity.Entities;

namespace EntitiesBT.Sample
{
    [GenerateAuthoringComponent]
    public struct ComponentVariableData : IComponentData
    {
        public float FloatValue;
        public int IntValue;
        public long LongValue;
    }
}
