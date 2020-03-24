using System;
using EntitiesBT.Components;
using EntitiesBT.Core;
using EntitiesBT.DebugView;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace EntitiesBT.Sample
{
    public class IsEntityPositionInBox : BTNode<IsEntityPositionInBoxNode>
    {
        public BoxCollider Box;
        
        protected override void Build(ref IsEntityPositionInBoxNode data, BlobBuilder _, ITreeNode<INodeDataBuilder>[] __)
        {
            // rotation is not count into.
            data.Bounds = new Bounds(Box.center + transform.position, Box.size);
        }
    }
    
    [Serializable]
    [BehaviorNode("404DBF2F-A83B-4FF8-B755-F2A6D6836793")]
    public struct IsEntityPositionInBoxNode : INodeData
    {
        public Bounds Bounds;

        [ReadOnly(typeof(Translation))]
        public NodeState Tick(int index, INodeBlob blob, IBlackboard bb)
        {
            var translation = bb.GetData<Translation>();
            return Bounds.Contains(translation.Value) ? NodeState.Success : NodeState.Failure;
        }

        public void Reset(int index, INodeBlob blob, IBlackboard blackboard)
        {
        }
    }
    
    public class IsEntityPositionInBoxNodeDebugView : BTDebugView<IsEntityPositionInBoxNode> {}
}
