// automatically generate from `OdinNodeComponentTemplate.cs`
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;
using EntitiesBT.Variant;

namespace EntitiesBT.Components.Odin
{
    public class OdinRepeatTimes : OdinNode<EntitiesBT.Nodes.RepeatTimesNode>
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

#endif
