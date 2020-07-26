using Unity.Entities;

namespace EntitiesBT.Sample
{
    [GenerateAuthoringComponent]
    public struct SampleTimeDelayData : IComponentData
    {
        public float OneSecond;
        public float TwoSeconds;
        public float ThreeSeconds;
    }
}
