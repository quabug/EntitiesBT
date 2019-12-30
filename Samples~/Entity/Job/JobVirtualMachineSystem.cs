using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

namespace EntitiesBT.Sample
{
    public class JobVirtualMachineSystem : JobComponentSystem
    {
        protected override unsafe JobHandle OnUpdate(JobHandle inputDeps)
        {
            var deltaTime = Time.DeltaTime;
            return Entities.WithoutBurst().ForEach((ref CustomBlackboard bb, ref NodeBlobRef blob, ref Translation translation) =>
            {
                bb.TickDeltaTime = new TickDeltaTime(TimeSpan.FromSeconds(deltaTime));
                bb.Translation = (Translation*) UnsafeUtility.AddressOf(ref translation);
                VirtualMachine.Tick(blob, bb);
            }).Schedule(inputDeps);
        }
    }
}
