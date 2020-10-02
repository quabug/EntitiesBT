using System;
using EntitiesBT.Entities;
using Runtime;
using Unity.Entities;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/SetBehaviorTree")]
    [Serializable]
    public struct SetBehaviorTree : IFlowNode
    {
        [PortDescription("", Description = "Trigger the GameObject disable state.")]
        public InputTriggerPort Input;

        [PortDescription(Runtime.ValueType.Entity)]
        public InputDataPort BehaviorTreeEntity;

        [PortDescription(Runtime.ValueType.Entity)]
        public InputDataPort TargetEntity;

        [PortDescription(Runtime.ValueType.Int)]
        public InputDataPort Order;

        [PortDescription(Runtime.ValueType.Bool)]
        public InputDataPort CloneBehaviorTree;

        [PortDescription(Runtime.ValueType.Bool)]
        public InputDataPort Debug;

        public Execution Execute<TCtx>(TCtx ctx, InputTriggerPort port) where TCtx : IGraphInstance
        {
            var dstManager = ctx.EntityManager;

            var behaviorTree = ctx.ReadEntity(BehaviorTreeEntity);
            if (ctx.ReadBool(CloneBehaviorTree)) behaviorTree = dstManager.CloneBehaviorTree(behaviorTree);

            var target = ctx.ReadEntity(TargetEntity);
            if (target == Entity.Null) target = ctx.CurrentEntity;

            var order = ctx.ReadInt(Order);

            if (!dstManager.HasComponent<BehaviorTreeBufferElement>(target))
                dstManager.AddBuffer<BehaviorTreeBufferElement>(target);

            if (!dstManager.HasComponent<CurrentBehaviorTreeComponent>(target))
                dstManager.AddComponent<CurrentBehaviorTreeComponent>(target);

            if (dstManager.HasComponent<BehaviorTreeTargetComponent>(behaviorTree))
                dstManager.SetComponentData(behaviorTree, new BehaviorTreeTargetComponent {Value = target});
            else
                dstManager.AddComponentData(behaviorTree, new BehaviorTreeTargetComponent {Value = target});

            if (dstManager.HasComponent<BehaviorTreeOrderComponent>(behaviorTree))
                dstManager.SetComponentData(behaviorTree, new BehaviorTreeOrderComponent {Value = order});
            else
                dstManager.AddComponentData(behaviorTree, new BehaviorTreeOrderComponent {Value = order});

            if (ctx.ReadBool(Debug) && !dstManager.HasComponent<BehaviorTreeDebug>(behaviorTree))
                dstManager.AddComponentData(target, new BehaviorTreeDebug());

            return Execution.Done;
        }
    }
}