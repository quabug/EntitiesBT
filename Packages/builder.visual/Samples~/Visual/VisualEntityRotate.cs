using System;
using EntitiesBT.Core;
using EntitiesBT.Entities;
using EntitiesBT.Variant;
using Runtime;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using ValueType = Runtime.ValueType;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/EntityRotate")]
    [Serializable]
    public struct VisualEntityRotate : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription(ValueType.Float3)] public InputDataPort Axis;
        [PortDescription(ValueType.Float)] public InputDataPort RadianPerSecond;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<VisualEntityRotateNode>(BuildImpl);

            unsafe void BuildImpl(BlobBuilder blobBuilder, ref VisualEntityRotateNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                @this.Axis.ToVariantReader<float3>(instance, definition)
                    .Allocate(ref blobBuilder, ref data.Axis, self, builders)
                ;
                @this.RadianPerSecond.ToVariantReader<float>(instance, definition)
                    .Allocate(ref blobBuilder, ref data.RadianPerSecond, self, builders)
                ;
            }
        }

        public Execution Execute<TCtx>(TCtx ctx, InputTriggerPort port) where TCtx : IGraphInstance => Execution.Done;
    }

    [Serializable]
    [BehaviorNode("6A4E0F98-7305-439B-A68C-CA42AAC51C34")]
    public struct VisualEntityRotateNode : INodeData
    {
        public BlobVariantReader<float3> Axis;
        public BlobVariantReader<float> RadianPerSecond;

        public NodeState Tick<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard bb)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
            ref var rotation = ref bb.GetDataRef<Rotation>();
            var deltaTime = bb.GetData<BehaviorTreeTickDeltaTime>();
            var axis = Axis.Read(index, ref blob, ref bb);
            var radianPerSecond = RadianPerSecond.Read(index, ref blob, ref bb);
            rotation.Value = math.mul(
                math.normalize(rotation.Value)
              , quaternion.AxisAngle(axis, radianPerSecond * deltaTime.Value)
            );
            return NodeState.Running;
        }

        public void Reset<TNodeBlob, TBlackboard>(int index, ref TNodeBlob blob, ref TBlackboard blackboard)
            where TNodeBlob : struct, INodeBlob
            where TBlackboard : struct, IBlackboard
        {
        }
    }
}