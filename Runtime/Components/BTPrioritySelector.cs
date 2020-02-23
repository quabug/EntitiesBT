using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Nodes;

namespace EntitiesBT.Components
{
    public class BTPrioritySelector : BTNode<PrioritySelectorNode>
    {
        public int[] Weights;

        public override int Size => PrioritySelectorNode.Size(Weights.Length);

        protected override void Build(ref PrioritySelectorNode data, ITreeNode<INodeDataBuilder>[] builders)
        {
            data.Weights.FromArrayUnsafe(Weights);
        }

        protected override void Update()
        {
            base.Update();
            Array.Resize(ref Weights, Children.Count());
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            Array.Resize(ref Weights, Children.Count());
        }
    }
}
