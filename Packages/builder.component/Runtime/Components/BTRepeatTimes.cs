// automatically generate from `NodeComponentTemplateCode.cs`
using EntitiesBT.Core;
using Unity.Entities;

namespace EntitiesBT.Components
{
    public class BTRepeatTimes : BTNode<EntitiesBT.Nodes.RepeatTimesNode>
    {
        public System.Int32 TickTimes;
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatTimesNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.TickTimes = TickTimes;
            data.BreakStates = BreakStates;
        }
    }
}
