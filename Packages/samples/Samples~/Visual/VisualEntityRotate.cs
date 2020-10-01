using System;
using EntitiesBT.Core;
using EntitiesBT.Sample;
using Runtime;
using Unity.Entities;
using Unity.Mathematics;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Build/EntityRotate")]
    [Serializable]
    public struct VisualEntityRotate : INode, IVisualBuilderNode
    {
        [PortDescription("")]
        public InputTriggerPort Parent;

        public float3 Axis;
        public float RadianPerSecond;

        public INodeDataBuilder GetBuilder(GraphDefinition definition)
        {
            return new VisualBuilder<EntityRotateNode>(BuildImpl);
        }

        public void BuildImpl(BlobBuilder blobBuilder, ref EntityRotateNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.Axis = Axis;
            data.RadianPerSecond = RadianPerSecond;
        }
    }
}