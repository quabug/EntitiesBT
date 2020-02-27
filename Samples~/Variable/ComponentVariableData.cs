using Unity.Entities;

namespace EntitiesBT.Sample
{
    [GenerateAuthoringComponent]
    public struct ComponentVariableData : IComponentData
    {
        public int InputValue;
        public float OutputValue;
    }
}
