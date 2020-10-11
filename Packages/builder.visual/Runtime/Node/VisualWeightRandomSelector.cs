// automatically generate from `VisualNodeTemplateCode.cs`
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using EntitiesBT.Components;
using Unity.Entities;
using System;
using Runtime;

namespace EntitiesBT.Builder.Visual
{
    [NodeSearcherItem("EntitiesBT/Node/WeightRandomSelectorNode")]
    [Serializable]
    public class VisualWeightRandomSelector : IVisualBuilderNode
    {
        [PortDescription("")] public InputTriggerPort Parent;
        [PortDescription("")] public OutputTriggerPort Children;

        public System.Single Sum;
        public System.Single[] Weights;

        public INodeDataBuilder GetBuilder(GraphInstance instance, GraphDefinition definition)
        {
            var @this = this;
            return new VisualBuilder<WeightRandomSelectorNode>(BuildImpl, Children.ToBuilderNode(instance, definition));
            void BuildImpl(BlobBuilder blobBuilder, ref EntitiesBT.Nodes.WeightRandomSelectorNode data, INodeDataBuilder self, ITreeNode<INodeDataBuilder>[] builders)
            {
                data.Sum = Sum;
                blobBuilder.AllocateArray(ref data.Weights, Weights);
            }
        }
    }
}
