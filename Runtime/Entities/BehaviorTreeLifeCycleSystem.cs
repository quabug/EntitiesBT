using System;
using System.Reflection;
using Unity.Entities;

namespace EntitiesBT.Entities
{
    [UpdateBefore(typeof(VirtualMachineSystem))]
    public class BehaviorTreeLifeCycleSystem : SystemBase
    {
        struct LastTargetComponent : ISystemStateComponentData
        {
            public Entity Target;
            public NodeBlobRef Blob;
        }

        private EntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var ecb = _entityCommandBufferSystem.CreateCommandBuffer();
            
            Entities
                .WithoutBurst()
                .WithNone<LastTargetComponent>()
                .ForEach((Entity entity, in BlackboardDataQuery query, in BehaviorTreeComponent bt, in BehaviorTreeTargetComponent target, in BehaviorTreeOrderComponent order) =>
                {
                    ecb.AddComponent(entity, new LastTargetComponent {Target = target.Value, Blob = bt.Blob});
                    BindBehaviorTree(ecb, entity, bt, query, target.Value, order.Value);
                }).Run();
            
            Entities
                .WithoutBurst()
                .WithChangeFilter<BehaviorTreeTargetComponent>()
                .ForEach((Entity entity, ref LastTargetComponent lastTarget, in BlackboardDataQuery query, in BehaviorTreeComponent bt, in BehaviorTreeTargetComponent target, in BehaviorTreeOrderComponent order) =>
                {
                    if (lastTarget.Target != target.Value)
                    {
                        UnbindBehaviorTree(lastTarget.Target, lastTarget.Blob);
                        BindBehaviorTree(ecb, entity, bt, query, target.Value, order.Value);
                        lastTarget.Target = target.Value;
                    }
                }).Run();
            
            Entities
                .WithoutBurst()
                .WithNone<BehaviorTreeTargetComponent>()
                .ForEach((Entity entity, in LastTargetComponent lastTarget) =>
            {
                UnbindBehaviorTree(lastTarget.Target, lastTarget.Blob);
                lastTarget.Blob.BlobRef.Dispose();
                ecb.RemoveComponent<LastTargetComponent>(entity);
            }).Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        void BindBehaviorTree(EntityCommandBuffer ecb, in Entity entity, in BehaviorTreeComponent bt, in BlackboardDataQuery query, in Entity target, in int order)
        {
            if (bt.AutoCreation != AutoCreateType.None)
            {
                foreach (var componentType in query.Set)
                {
                    if (EntityManager.HasComponent(target, componentType)) continue;
                    var shouldCreate =
                        bt.AutoCreation.HasFlagFast(AutoCreateType.ReadOnly) && componentType.AccessModeType == ComponentType.AccessMode.ReadOnly
                        || bt.AutoCreation.HasFlagFast(AutoCreateType.ReadWrite) && componentType.AccessModeType == ComponentType.AccessMode.ReadWrite
                        || bt.AutoCreation.HasFlagFast(AutoCreateType.BehaviorTreeComponent) && TypeManager.GetType(componentType.TypeIndex)?.GetCustomAttribute<BehaviorTreeComponentAttribute>() != null
                    ;
                    if (shouldCreate)
                    {
                        var typeInfo = TypeManager.GetTypeInfo(componentType.TypeIndex);
                        switch (typeInfo.Category)
                        {
                            case TypeManager.TypeCategory.ComponentData:
                                ecb.AddComponent(target, componentType);
                                break;
                            case TypeManager.TypeCategory.BufferData:
                                ecb.AddBuffer(target, componentType);
                                break;
                        }
                    }
                }
            }

            var buffers = EntityManager.GetBuffer<BehaviorTreeBufferElement>(target);
            var orderedIndex = 0;
            // TODO: binary search?
            while (orderedIndex < buffers.Length && buffers[orderedIndex].Order < order) orderedIndex++;
            var element = new BehaviorTreeBufferElement
            {
                Order = order
              , NodeBlob = bt.Blob
              , QueryMask = EntityManager.GetEntityQueryMask(query.Query)
              , RuntimeThread = bt.Thread.ToRuntimeThread()
              , BehaviorTree = entity
              , Dependency = query.Query.GetDependency()
            };
            buffers.Insert(orderedIndex, element);
        }

        void UnbindBehaviorTree(in Entity target, in NodeBlobRef blob)
        {
            if (!EntityManager.HasComponent<BehaviorTreeBufferElement>(target))
            {
                // TODO: log error message?
                return;
            }

            var buffers = EntityManager.GetBuffer<BehaviorTreeBufferElement>(target);
            for (var i = buffers.Length - 1; i >= 0; i--)
            {
                if (buffers[i].NodeBlob == blob)
                {
                    buffers.RemoveAt(i);
                    break;
                }
            }
            
            // TODO: log not found?
        }
    }
}