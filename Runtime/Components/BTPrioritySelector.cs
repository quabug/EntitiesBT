using System;
using System.Linq;
using EntitiesBT.Core;
using EntitiesBT.Nodes;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTPrioritySelector : BTNode<PrioritySelectorNode>
    {
        public int[] Weights;

        protected override void Build(ref PrioritySelectorNode data, BlobBuilder blobBuilder, ITreeNode<INodeDataBuilder>[] builders)
        {
            blobBuilder.AllocateArray(ref data.Weights, Weights);
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
