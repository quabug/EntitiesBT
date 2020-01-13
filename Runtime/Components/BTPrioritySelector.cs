using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTPrioritySelector : BTNode<PrioritySelectorNode, PrioritySelectorNode.Data>
    {
        public int[] Weights;

        public override int Size => PrioritySelectorNode.Data.Size(Weights.Length);

        protected override void Build(ref PrioritySelectorNode.Data data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.Weights.FromArrayUnsafe(Weights);
        }

        protected override void OnTransformChildrenChanged()
        {
            base.OnTransformChildrenChanged();
            Array.Resize(ref Weights, Children.Count());
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Array.Resize(ref Weights, Children.Count());
        }
    }
}
