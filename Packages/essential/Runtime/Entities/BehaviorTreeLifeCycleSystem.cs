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

        // TODO: sync-points optimization?
        protected override void OnUpdate()
        {
            // create
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithNone<LastTargetComponent>()
                .ForEach((Entity entity, in BlackboardDataQuery query, in BehaviorTreeComponent bt, in BehaviorTreeTargetComponent target, in BehaviorTreeOrderComponent order) =>
                {
                    var blob = new NodeBlobRef(bt.Blob.BlobRef.Clone());
                    EntityManager.AddComponentData(entity, new LastTargetComponent {Target = target.Value, Blob = blob});
                    BindBehaviorTree(entity, bt, query, target.Value, order.Value, blob);
                }).Run();

            // update
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithChangeFilter<BehaviorTreeTargetComponent>()
                .ForEach((Entity entity, ref LastTargetComponent lastTarget, in BlackboardDataQuery query, in BehaviorTreeComponent bt, in BehaviorTreeTargetComponent target, in BehaviorTreeOrderComponent order) =>
                {
                    if (lastTarget.Target != target.Value)
                    {
                        UnbindBehaviorTree(entity, lastTarget.Target);
                        BindBehaviorTree(entity, bt, query, target.Value, order.Value, lastTarget.Blob);
                        lastTarget.Target = target.Value;
                    }
                }).Run();

            // delete
            Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .WithNone<BehaviorTreeTargetComponent>()
                .ForEach((Entity entity, in LastTargetComponent lastTarget) =>
            {
                UnbindBehaviorTree(entity, lastTarget.Target);
                lastTarget.Blob.BlobRef.Dispose();
                EntityManager.RemoveComponent<LastTargetComponent>(entity);
            }).Run();
        }

        void BindBehaviorTree(Entity behaviorTreeEntity, in BehaviorTreeComponent bt, in BlackboardDataQuery query, Entity target, int order, NodeBlobRef blob)
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
                                EntityManager.AddComponent(target, componentType);
                                break;
                            case TypeManager.TypeCategory.BufferData:
                                EntityManager.AddComponent(target, ComponentType.ReadWrite(componentType.TypeIndex));
                                break;
                        }
                    }
                }
            }

            var buffer = EntityManager.GetBuffer<BehaviorTreeBufferElement>(target);
            var orderedIndex = 0;
            // TODO: binary search?
            while (orderedIndex < buffer.Length && buffer[orderedIndex].Order < order) orderedIndex++;
            var element = new BehaviorTreeBufferElement
            (
                order
              , bt.Thread.ToRuntimeThread()
              , blob
              , EntityManager.GetEntityQueryMask(query.Query)
              , behaviorTreeEntity
              , query.Query.GetDependency()
            );
            buffer.Insert(orderedIndex, element);
        }

        void UnbindBehaviorTree(Entity behaviorTreeEntity, Entity targetEntity)
        {
            if (!EntityManager.HasComponent<BehaviorTreeBufferElement>(targetEntity))
            {
                // TODO: log error message?
                return;
            }

            var buffer = EntityManager.GetBuffer<BehaviorTreeBufferElement>(targetEntity);
            for (var i = buffer.Length - 1; i >= 0; i--)
            {
                if (buffer[i].BehaviorTree == behaviorTreeEntity)
                {
                    buffer.RemoveAt(i);
                    break;
                }
            }
            
            // TODO: log not found?
        }
    }
}