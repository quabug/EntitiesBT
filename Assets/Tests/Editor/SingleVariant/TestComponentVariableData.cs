using Unity.Entities;

namespace EntitiesBT.Test
{
    public struct TestComponentVariableData : IComponentData
    {
        public float FloatValue;
        public int IntValue;
        public long LongValue;
    }
}