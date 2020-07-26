using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Entities
{
    [GenerateAuthoringComponent]
    public struct BehaviorTreeRandomSeed : IComponentData
    {
        public uint Value;
    }
    
    [BehaviorTreeComponent]
    public struct BehaviorTreeRandom : IComponentData
    {
        public Random Value;
    }
    
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BehaviorTreeInitRandomSystem : SystemBase
    {
        private struct ExistTag : ISystemStateComponentData {}
        private EntityCommandBufferSystem _ECB;

        protected override void OnCreate()
        {
            _ECB = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            InitByRandomSeed();
            InitByCustomSeed();
            Destroy();
            _ECB.AddJobHandleForProducer(Dependency);
        }

        void InitByRandomSeed()
        {
            var ecb = _ECB.CreateCommandBuffer().ToConcurrent();
            var randomSeed = new Random((uint)System.Environment.TickCount);
            Entities.WithNone<ExistTag, BehaviorTreeRandomSeed>()
                .ForEach((Entity entity, int nativeThreadIndex, ref BehaviorTreeRandom random) =>
                {
                    random.Value.InitState(randomSeed.NextUInt());
                    ecb.AddComponent<ExistTag>(nativeThreadIndex, entity);
                }).ScheduleParallel();
        }

        void InitByCustomSeed()
        {
            var ecb = _ECB.CreateCommandBuffer().ToConcurrent();
            Entities.WithNone<ExistTag>()
                .ForEach((Entity entity, int nativeThreadIndex, ref BehaviorTreeRandom random, ref BehaviorTreeRandomSeed seed) => {
                    random.Value.InitState(seed.Value);
                    ecb.AddComponent<ExistTag>(nativeThreadIndex, entity);
                }).ScheduleParallel();
        }

        void Destroy()
        {
            var ecb = _ECB.CreateCommandBuffer().ToConcurrent();
            Entities.WithNone<BehaviorTreeRandom>().WithAll<ExistTag>()
                .ForEach((Entity entity, int nativeThreadIndex) =>
                    ecb.RemoveComponent<ExistTag>(nativeThreadIndex, entity))
                .ScheduleParallel()
            ;
        }
    }
}
