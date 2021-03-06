// automatically generate from `OdinNodeComponentTemplate.cs`
#if ODIN_INSPECTOR

using EntitiesBT.Core;
using Unity.Entities;
using Sirenix.Serialization;
using System;
using EntitiesBT.Variant;
using Sirenix.OdinInspector;

namespace EntitiesBT.Components.Odin
{
    public class OdinRepeatDuration : OdinNode<EntitiesBT.Nodes.RepeatDurationNode>
    {
        public System.Single CountdownSeconds;
        public EntitiesBT.Core.NodeState BreakStates;
        protected override void Build(ref EntitiesBT.Nodes.RepeatDurationNode data, BlobBuilder builder, ITreeNode<INodeDataBuilder>[] tree)
        {
            data.CountdownSeconds = CountdownSeconds;
            data.BreakStates = BreakStates;
        }
    }
}

#endif
